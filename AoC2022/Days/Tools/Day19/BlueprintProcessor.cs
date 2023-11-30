using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day19
{

    internal enum Product
    {
        Ore,
        Clay,
        Obsidian,
        Geode,
    }

    internal class BlueprintProcessor
    {
        private Dictionary<Product, ProductDetail> _products;
        public int ID { get; private set; }
        public string DebugProtoceol { get; private set; }

        private const string itemAmountGroup = "itemAmount";
        private const string ItemNameGroup = "itemName";
        private const string productNameGroup = "product";

        public BlueprintProcessor(string blueprint)
        {
            LoadBlueprint(blueprint);
        }

        private void LoadBlueprint(string blueprint)
        {
            _products = new Dictionary<Product, ProductDetail>();
            var segments = blueprint.Split('.', ':');
            ID = int.Parse(segments[0].Split(' ').Last());
            var regEx = new Regex(@"Each (?<product>\w+) robot costs (?:(?: and )?(?<itemAmount>\d+) (?<itemName>\w+))*");
            foreach (var robot in segments.Skip(1))
            {
                var info = regEx.Match(robot.Trim());
                if (!info.Success) continue;
                Product product = (Product)Enum.Parse(typeof(Product), info.Groups[productNameGroup].Value, true);
                var productDetail = new ProductDetail();
                for (int i = 0; i < info.Groups[itemAmountGroup].Captures.Count; i++)
                {
                    Product costItem = (Product)Enum.Parse(typeof(Product), info.Groups[ItemNameGroup].Captures[i].Value, true);
                    int itemAmount = int.Parse(info.Groups[itemAmountGroup].Captures[i].Value);
                    productDetail.RobotCost.Add(costItem, itemAmount);
                }
                _products.Add(product, productDetail);
            }
        }

        public int RunBlueprint()
        {
            Reset();
            var protocol = new StringBuilder();
            for (int i = 0; i < 24; ++i)
            {
                protocol.AppendLine($"== Minute {i + 1} ==");
                Tick(protocol);
            }
            DebugProtoceol = protocol.ToString();
            return _products[Product.Geode].Inventory * ID;
        }

        public Task<int> RunBlueprintAsync()
        {
            var task = new Task<int>(RunBlueprint);
            task.Start();
            return task;
        }

        private void Reset()
        {
            foreach (var productEntry in _products)
            {
                productEntry.Value.Inventory = 0;
                productEntry.Value.MiningRobots = productEntry.Key == Product.Ore ? 1 : 0;
            }
        }

        private void Tick(StringBuilder protocol)
        {
            Product? addedRobot = null;
            foreach (var product in Enum.GetValues(typeof(Product)).Cast<Product>().Reverse())
            {
                if (!CanProduce(product)) continue;

                protocol.Append("Spend ");
                addedRobot = product;
                bool isFirst = true;
                foreach (var costItem in _products[product].RobotCost)
                {
                    _products[costItem.Key].Inventory -= costItem.Value;
                    protocol.Append(string.Concat(!isFirst ? "and " : "", costItem.Value, " ", costItem.Key.ToString().ToLower(), " "));
                    isFirst = false;
                }
                protocol.AppendLine($"to start building a {addedRobot.Value.ToString().ToLower()}-collecting robot.");
                break;
            }
            foreach (var product in _products)
            {
                product.Value.Inventory += product.Value.MiningRobots;
                if (product.Value.MiningRobots > 0)
                {
                    bool plural = product.Value.MiningRobots > 1;
                    protocol.AppendLine($"{product.Value.MiningRobots} {product.Key.ToString().ToLower()}-collecting robot{(plural ? "s" : "")} collect{(plural ? "" : "s")} {product.Value.MiningRobots} {product.Key.ToString().ToLower()}; you now have {product.Value.Inventory} {product.Key.ToString().ToLower()}.");
                }
            }
            if (addedRobot.HasValue)
            {
                _products[addedRobot.Value].MiningRobots++;
                protocol.AppendLine($"The new {addedRobot.Value.ToString().ToLower()}-collecting robot is ready; you now have {_products[addedRobot.Value].MiningRobots} of them.");
            }
            protocol.AppendLine();
        }

        private bool CanProduce(Product product)
        {
            foreach (var costItem in _products[product].RobotCost)
                if (_products[costItem.Key].Inventory < costItem.Value)
                    return false;
            return true;
        }
    }

    internal class ProductDetail
    {
        public Dictionary<Product, int> RobotCost { get; } = new Dictionary<Product, int>();
        public int MiningRobots { get; set; }
        public int Inventory { get; set; }
    }
}
