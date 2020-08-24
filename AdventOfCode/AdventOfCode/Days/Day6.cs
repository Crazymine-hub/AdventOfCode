using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Days.Classes.Day6;

namespace AdventOfCode.Days
{
    class Day6 : IDay
    {
        private List<OrbitInfo> orbitDetails;
        private List<OrbitObject> totalObjects;
        private OrbitObject COM = new OrbitObject() { Name = "COM" };

        public string Solve(string input, bool part2)
        {
            //input = "COM)B\nB)C\nC)D\nD)E\nE)F\nB)G\nG)H\nD)I\nE)J\nJ)K\nK)L";
            //input = "COM)B\nB)C\nC)D\nD)E\nE)F\nB)G\nG)H\nD)I\nE)J\nJ)K\nK)L\nK)YOU\nI)SAN";
            string[] orbits = input.Split('\n');
            orbitDetails = new List<OrbitInfo>();
            totalObjects = new List<OrbitObject>();
            foreach (string orbit in orbits)
            {
                string[] orbitDetail = orbit.Split(')');
                orbitDetails.Add(new OrbitInfo() { From = orbitDetail[0], To = orbitDetail[1] });
            }
            GetChildOrbitals(COM, 0);
            if (part2)
            {
                OrbitObject start = GetObjectByName("YOU");
                OrbitObject end = GetObjectByName("SAN");
                if (start == null || end == null)
                    throw new Exception("Start or Target not found!");
                return "Jumps:" + TraceJumpPath(start, end);
            }
            else
                return "Orbits: " + BuildChecksum();
        }

        private int TraceJumpPath(OrbitObject start, OrbitObject target)
        {
            OrbitObject[] targetTree = GetTree(target);
            OrbitObject currObject = start;
            List<OrbitObject> fullPath = new List<OrbitObject>();
            while (!targetTree.Contains(currObject))
            {
                currObject = currObject.ParentObject;
                fullPath.Add(currObject);
            }

            bool intersectionFound = false;
            for (int i = targetTree.Length - 1; i >= 1; i--)
            {
                if (intersectionFound)
                {
                    fullPath.Add(targetTree[i]);
                }
                else
                {
                    if(targetTree[i] == currObject)
                        intersectionFound = true;
                }
            }
            return fullPath.Count-1;
        }

        private int BuildChecksum()
        {
            int orbitCount = 0;
            foreach (OrbitObject orbitObject in totalObjects)
            {
                orbitCount += orbitObject.OrbitCount;
            }
            return orbitCount;
        }

        private OrbitObject GetObjectByName(string name)
        {
            foreach (OrbitObject currObject in totalObjects)
                if (currObject.Name == name)
                    return currObject;
            return null;
        }

        private OrbitInfo[] FindOrbitingObjects(string centerName)
        {
            List<OrbitInfo> foundOrbits = new List<OrbitInfo>();
            foreach (OrbitInfo orbit in orbitDetails)
            {
                if (orbit.From == centerName)
                    foundOrbits.Add(orbit);
            }
            return foundOrbits.ToArray();
        }

        private void GetChildOrbitals(OrbitObject currObject, int indirectOrbits)
        {
            OrbitInfo[] orbits = FindOrbitingObjects(currObject.Name);
            List<OrbitObject> children = new List<OrbitObject>();
            foreach (OrbitInfo orbit in orbits)
            {
                OrbitObject child = new OrbitObject();
                child.Name = orbit.To;
                child.OrbitCount = indirectOrbits + 1;
                child.ParentObject = currObject;
                children.Add(child);
                totalObjects.Add(child);
                GetChildOrbitals(child, indirectOrbits + 1);
            }
            currObject.ChildObjects = children.ToArray();
        }

        private OrbitObject[] GetTree(OrbitObject start)
        {
            List<OrbitObject> tree = new List<OrbitObject>();
            OrbitObject currObject = start;
            while (currObject.ParentObject != null)
            {
                tree.Add(currObject);
                currObject = currObject.ParentObject;
            }
            tree.Add(currObject);
            return tree.ToArray();
        }
    }
}
