using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework
{
    public class TAStarMapNode
    {
        public class NodeConnection
        {
            public TAStarMapNode ConnectedNode;
            public float Weight;
        }

        public Vector3Int Coordinate;
        public List<NodeConnection> Connections = new List<NodeConnection>();

        public void AddNearbyNode(TAStarMapNode aNode, float aWeight)
        {
            NodeConnection con = this.Connections.Find((v) =>
            {
                return v.ConnectedNode.Coordinate.Equals(aNode.Coordinate);
            });
            if (con != null)
            {
                return;
            }
            NodeConnection newConn = new NodeConnection();
            newConn.ConnectedNode = aNode;
            newConn.Weight = aWeight;
            this.Connections.Add(newConn);
        }
    }

    public class TAStarManager
    {
        public class TAStarPathNode
        {
            public Vector3Int Coordinate;
            public TAStarPathNode Previous;
            public float G;
        }

        public Dictionary<Vector3Int, TAStarMapNode> MapData = new Dictionary<Vector3Int, TAStarMapNode>();
        public float EstimatedWeight = 1;

        public bool AddMapNode(TAStarMapNode aNode)
        {
            if (this.MapData.ContainsKey(aNode.Coordinate) == true)
            {
                return false;
            }
            this.MapData.Add(aNode.Coordinate, aNode);
            return true;
        }

        public TAStarMapNode FindNode(Vector3Int aCoordinate)
        {
            TAStarMapNode node = null;
            this.MapData.TryGetValue(aCoordinate, out node);
            return node;
        }

        public List<Vector3Int> CalculatePath(Vector3Int aStart, Vector3Int aEnd)
        {
            if (this.MapData.ContainsKey(aStart) == false)
            {
                return null;
            }
            if (this.MapData.ContainsKey(aEnd) == false)
            {
                return null;
            }

            List<TAStarMapNode> openList = new List<TAStarMapNode>();
            List<TAStarMapNode> closeList = new List<TAStarMapNode>();
            List<TAStarPathNode> pathList = new List<TAStarPathNode>();

            TAStarMapNode endNode = this.FindNode(aEnd);
            TAStarMapNode curNode = this.FindNode(aStart);
            openList.Add(curNode);

            TAStarPathNode firstPathNode = new TAStarPathNode();
            firstPathNode.Coordinate = curNode.Coordinate;
            firstPathNode.Previous = null;
            firstPathNode.G = 0;
            pathList.Add(firstPathNode);

            TAStarPathNode endPathNode = null;
            while (openList.Count > 0)
            {
                //找一个权重最小的节点出来
                float tmpF = float.MaxValue;
                foreach (TAStarMapNode node in openList)
                {

                    TAStarPathNode tmpPathNode = pathList.Find((v) => { return v.Coordinate.Equals(node.Coordinate); });
                    float dx = aEnd.x - tmpPathNode.Coordinate.x;
                    float dy = aEnd.y - tmpPathNode.Coordinate.y;
                    float dz = aEnd.z - tmpPathNode.Coordinate.z;
                    float G = tmpPathNode.G;
                    float H = (dx * dx + dy * dy + dz * dz) * this.EstimatedWeight;
                    if (tmpF > G + H)
                    {
                        tmpF = G + H;
                        curNode = node;
                    }
                }

                foreach (var conn in curNode.Connections)
                {
                    TAStarMapNode tmpNode = conn.ConnectedNode;
                    if (closeList.Contains(tmpNode) == true)
                    {
                        continue;
                    }

                    if (openList.Contains(tmpNode) == true)
                    {
                        TAStarPathNode curPathNode = pathList.Find((v) => { return v.Coordinate.Equals(curNode.Coordinate); });
                        TAStarPathNode targetPathNode = pathList.Find((v) => { return v.Coordinate.Equals(tmpNode.Coordinate); });

                        float curG = curPathNode.G + conn.Weight;
                        float tarG = targetPathNode.G;
                        if (tarG > curG)
                        {
                            targetPathNode.Previous = curPathNode;
                            targetPathNode.G = curG;
                        }
                    }
                    else
                    {
                        TAStarPathNode curPathNode = pathList.Find((v) => { return v.Coordinate.Equals(curNode.Coordinate); });

                        //从来没有到过的新节点
                        TAStarPathNode pathNode = new TAStarPathNode();
                        pathNode.Coordinate = tmpNode.Coordinate;
                        pathNode.Previous = curPathNode;
                        pathNode.G = curPathNode.G + conn.Weight;
                        pathList.Add(pathNode);
                        openList.Add(tmpNode);
                    }
                }

                closeList.Add(curNode);
                openList.Remove(curNode);

                if (curNode == endNode)
                {
                    endPathNode = pathList.Find((v) => { return v.Coordinate.Equals(curNode.Coordinate); });
                    break;
                }
            }

            if (endPathNode != null)
            {
                List<Vector3Int> path = new List<Vector3Int>();
                TAStarPathNode tmp = endPathNode;
                while (tmp != null)
                {
                    path.Add(tmp.Coordinate);
                    tmp = tmp.Previous;
                }
                path.Reverse();
                return path;
            }

            return null;
        }


        public List<Vector3Int> CalculatePathPunishment(Vector3Int aStart, Vector3Int aEnd)
        {
            if (this.MapData.ContainsKey(aStart) == false)
            {
                return null;
            }
            if (this.MapData.ContainsKey(aEnd) == false)
            {
                return null;
            }

            List<TAStarMapNode> openList = new List<TAStarMapNode>();
            List<TAStarMapNode> closeList = new List<TAStarMapNode>();
            List<TAStarPathNode> pathList = new List<TAStarPathNode>();

            TAStarMapNode endNode = this.FindNode(aEnd);
            TAStarMapNode curNode = this.FindNode(aStart);
            openList.Add(curNode);

            TAStarPathNode firstPathNode = new TAStarPathNode();
            firstPathNode.Coordinate = curNode.Coordinate;
            firstPathNode.Previous = null;
            firstPathNode.G = 0;
            pathList.Add(firstPathNode);

            TAStarPathNode endPathNode = null;
            TAStarMapNode previousNode = null;
            Vector3Int dir = Vector3Int.zero;
            while (openList.Count > 0)
            {
                //找一个权重最小的节点出来
                float tmpF = float.MaxValue;
                foreach (TAStarMapNode node in openList)
                {
                    //惩罚连续的直线
                    float G2 = 0;
                    if (previousNode != null)
                    {
                        Vector3Int tmpDir = node.Coordinate - previousNode.Coordinate;
                        if (tmpDir.Equals(dir) == true) { G2 = 0.5f; }
                    }

                    TAStarPathNode tmpPathNode = pathList.Find((v) => { return v.Coordinate.Equals(node.Coordinate); });
                    float G = tmpPathNode.G + G2;
                    float H = Mathf.Abs(aEnd.x - tmpPathNode.Coordinate.x)
                        + Mathf.Abs(aEnd.y - tmpPathNode.Coordinate.y)
                        + Mathf.Abs(aEnd.z - tmpPathNode.Coordinate.z);
                    H *= this.EstimatedWeight;
                    if (tmpF > G + H)
                    {
                        tmpF = G + H;
                        curNode = node;
                    }
                }

                {
                    //记录连续的直线
                    if (previousNode != null)
                    {
                        dir = curNode.Coordinate - previousNode.Coordinate;
                        if (dir.magnitude > 1) { dir = Vector3Int.zero; }
                    }
                    previousNode = curNode;
                }

                foreach (var conn in curNode.Connections)
                {
                    TAStarMapNode tmpNode = conn.ConnectedNode;
                    if (closeList.Contains(tmpNode) == true)
                    {
                        continue;
                    }

                    if (openList.Contains(tmpNode) == true)
                    {
                        TAStarPathNode curPathNode = pathList.Find((v) => { return v.Coordinate.Equals(curNode.Coordinate); });
                        TAStarPathNode targetPathNode = pathList.Find((v) => { return v.Coordinate.Equals(tmpNode.Coordinate); });

                        float curG = curPathNode.G + conn.Weight;
                        float tarG = targetPathNode.G;
                        if (tarG > curG)
                        {
                            targetPathNode.Previous = curPathNode;
                            targetPathNode.G = curG;
                        }
                    }
                    else
                    {
                        TAStarPathNode curPathNode = pathList.Find((v) => { return v.Coordinate.Equals(curNode.Coordinate); });

                        //从来没有到过的新节点
                        TAStarPathNode pathNode = new TAStarPathNode();
                        pathNode.Coordinate = tmpNode.Coordinate;
                        pathNode.Previous = curPathNode;
                        pathNode.G = curPathNode.G + conn.Weight;
                        pathList.Add(pathNode);
                        openList.Insert(0, tmpNode);
                    }
                }

                closeList.Add(curNode);
                openList.Remove(curNode);

                if (curNode == endNode)
                {
                    endPathNode = pathList.Find((v) => { return v.Coordinate.Equals(curNode.Coordinate); });
                    break;
                }
            }

            if (endPathNode != null)
            {
                List<Vector3Int> path = new List<Vector3Int>();
                TAStarPathNode tmp = endPathNode;
                while (tmp != null)
                {
                    path.Add(tmp.Coordinate);
                    tmp = tmp.Previous;
                }
                path.Reverse();
                return path;
            }

            return null;
        }
    }
}

