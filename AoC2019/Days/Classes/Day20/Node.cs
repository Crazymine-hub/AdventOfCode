﻿using AdventOfCode.Tools.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Classes.Day20
{
    class Node: BaseNode
    {
        public string PortalCode { get; set; } = "";
        public override char CharRepresentation => PortalCode.Length > 0 ? PortalCode[0] : '\0';

        public Node(int x, int y, char portalChar) : base(x, y)
        {
            if (portalChar >= 65 && portalChar <= 90)
                PortalCode = portalChar.ToString();
        }

        public override string ToString()
        {
            return PortalCode + base.ToString().Remove(0, 1);
        }
    }
}
