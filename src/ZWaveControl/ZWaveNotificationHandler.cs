using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenZWaveDotNet;

namespace BuildLightControl.ZWave
{
    public static class ZWaveNotificationHandler
    {
        private static List<Node> _nodes = new List<Node>();

        public static Node[] GetNodes()
        {
            return _nodes.ToArray();
        }

        public static void HandleNotification(ZWNotification notification, ZWManager manager)
        {
            var notificationType = notification.GetType();

            switch (notificationType)
            {
                case ZWNotification.Type.ValueAdded:
                {
                    var value = notification.GetValueID();
                    Debug.WriteLine("Node {0} Value Added: {1} - {2} - {3}", notification.GetNodeId(),
                        manager.GetValueLabel(value), GetValue(value, manager), manager.GetValueUnits(value));
                    var node = GetNode(notification.GetHomeId(), notification.GetNodeId());
                    if (node != null)
                    {
                        node.Values.Add(value);
                    }
                    break;
                }
                case ZWNotification.Type.ValueRemoved:
                {
                    var value = notification.GetValueID();
                    Debug.WriteLine("Node {0} Value Removed", notification.GetNodeId());
                    var node = GetNode(notification.GetHomeId(), notification.GetNodeId());
                    if (node != null)
                    {
                        node.Values.Remove(value);
                    }
                    break;
                }
                case ZWNotification.Type.ValueChanged:
                {
                    var value = notification.GetValueID();
                    Debug.WriteLine("Node {0} Value Changed: {1} - {2} - {3}", notification.GetNodeId(),
                        manager.GetValueLabel(value), GetValue(value, manager), manager.GetValueUnits(value));
                    break;
                }

                case ZWNotification.Type.ValueRefreshed:
                    Debug.WriteLine("Value Refreshed");
                    break;
                case ZWNotification.Type.Group:
                    Debug.WriteLine("Group");
                    break;
                case ZWNotification.Type.NodeNew:
                {
                    // if the node is not in the z-wave config this will be called first
                    var node = new Node
                    {
                        Id = notification.GetNodeId(),
                        HomeId = notification.GetHomeId()
                    };
                    Debug.WriteLine("Node New: {0}, Home: {1}", node.Id, node.HomeId);
                    _nodes.Add(node);
                    break;
                }
                case ZWNotification.Type.NodeAdded:
                {
                    // if the node is in the z-wave config then this will be the first node notification
                    if (GetNode(notification.GetHomeId(), notification.GetNodeId()) == null)
                    {
                        var node = new Node
                        {
                            Id = notification.GetNodeId(),
                            HomeId = notification.GetHomeId()
                        };
                        Debug.WriteLine("Node Added: {0}, Home: {1}", node.Id, node.HomeId);
                        _nodes.Add(node);
                    }
                    break;
                }

                case ZWNotification.Type.NodeRemoved:
                    Debug.WriteLine("NodeRemoved");
                    break;
                case ZWNotification.Type.NodeProtocolInfo:
                    Debug.WriteLine("NodeProtocolInfo");
                    break;
                case ZWNotification.Type.NodeNaming:
                {
                    var node = GetNode(notification.GetHomeId(), notification.GetNodeId());
                    if (node != null)
                    {
                        node.Name = manager.GetNodeName(node.HomeId, node.Id);
                        node.Manufacturer = manager.GetNodeManufacturerName(node.HomeId, node.Id);
                        node.Product = manager.GetNodeProductName(node.HomeId, node.Id);
                        node.Location = manager.GetNodeLocation(node.HomeId, node.Id);

                        Debug.WriteLine("Name: {0}, Manufacturer: {1}, Product: {2}, Location: {3}", node.Name,
                            node.Manufacturer, node.Product, node.Location);
                    }
                    break;
                }
                case ZWNotification.Type.NodeEvent:
                    Debug.WriteLine("Node Event");
                    break;
                case ZWNotification.Type.PollingDisabled:
                    Debug.WriteLine("Polling Disabled");
                    break;
                case ZWNotification.Type.PollingEnabled:
                    Debug.WriteLine("Polling Enabled");
                    break;
                case ZWNotification.Type.SceneEvent:
                    Debug.WriteLine("Scene Event");
                    break;
                case ZWNotification.Type.CreateButton:
                    Debug.WriteLine("Create Button");
                    break;
                case ZWNotification.Type.DeleteButton:
                    Debug.WriteLine("Delete Button");
                    break;
                case ZWNotification.Type.ButtonOn:
                    Debug.WriteLine("Button On");
                    break;
                case ZWNotification.Type.ButtonOff:
                    Debug.WriteLine("Button Off");
                    break;
                case ZWNotification.Type.DriverReady:
                    Debug.WriteLine("Driver Ready");
                    break;
                case ZWNotification.Type.DriverFailed:
                    Debug.WriteLine("Driver Failed");
                    break;
                case ZWNotification.Type.DriverReset:
                    Debug.WriteLine("Driver Reset");
                    break;
                case ZWNotification.Type.EssentialNodeQueriesComplete:
                    Debug.WriteLine("Essential Node Queries Complete");
                    break;
                case ZWNotification.Type.NodeQueriesComplete:
                    Debug.WriteLine("Node Queries Complete");
                    break;
                case ZWNotification.Type.AwakeNodesQueried:
                    Debug.WriteLine("Awake Nodes Queried");
                    break;
                case ZWNotification.Type.AllNodesQueried:
                    Debug.WriteLine("All Nodes Queried");
                    break;
                case ZWNotification.Type.AllNodesQueriedSomeDead:
                    Debug.WriteLine("All Nodes Queried Some Dead");
                    break;
                case ZWNotification.Type.Notification:
                    Debug.WriteLine("Notification");
                    break;
            }
        }

        private static Node GetNode(uint homeId, byte nodeId)
        {
            return _nodes.FirstOrDefault(n => n.Id == nodeId && n.HomeId == homeId);
        }


        public static string GetValue(ZWValueID value, ZWManager manager)
        {
            switch (value.GetType())
            {
                case ZWValueID.ValueType.Bool:
                    bool boolValue;
                    manager.GetValueAsBool(value, out boolValue);
                    return boolValue.ToString();
                case ZWValueID.ValueType.Byte:
                    byte byteValue;
                    manager.GetValueAsByte(value, out byteValue);
                    return byteValue.ToString();
                case ZWValueID.ValueType.Decimal:
                    decimal decimalValue;
                    manager.GetValueAsDecimal(value, out decimalValue);
                    return decimalValue.ToString();
                case ZWValueID.ValueType.Int:
                    int intValue;
                    manager.GetValueAsInt(value, out intValue);
                    return intValue.ToString();
                case ZWValueID.ValueType.List:
                    string[] listValues;
                    manager.GetValueListItems(value, out listValues);
                    string listValue = "";
                    if (listValues != null)
                    {
                        foreach (string s in listValues)
                        {
                            listValue += s;
                            listValue += "/";
                        }
                    }
                    return listValue;
                case ZWValueID.ValueType.Schedule:
                    return "Schedule";
                case ZWValueID.ValueType.Short:
                    short shortValue;
                    manager.GetValueAsShort(value, out shortValue);
                    return shortValue.ToString();
                case ZWValueID.ValueType.String:
                    string stringValue;
                    manager.GetValueAsString(value, out stringValue);
                    return stringValue;
                default:
                    return "";
            }
        }
    }
}