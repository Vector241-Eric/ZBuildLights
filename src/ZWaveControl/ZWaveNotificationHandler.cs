using System.Collections.Generic;
using System.Linq;
using NLog;
using OpenZWaveDotNet;

namespace ZWaveControl
{
    public static class ZWaveNotificationHandler
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
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
                    var addedValue = notification.GetValueID();
                    Log.Debug("Node {0} Value Added: {1} - {2} - {3}", notification.GetNodeId(),
                        manager.GetValueLabel(addedValue), GetValue(addedValue, manager),
                        manager.GetValueUnits(addedValue));
                    var vaNode = GetNode(notification.GetHomeId(), notification.GetNodeId());
                    if (vaNode != null)
                    {
                        vaNode.Values.Add(addedValue);
                    }
                    break;
                case ZWNotification.Type.ValueRemoved:
                    var removedValue = notification.GetValueID();
                    Log.Debug("Node {0} Value Removed", notification.GetNodeId());
                    var vrNode = GetNode(notification.GetHomeId(), notification.GetNodeId());
                    if (vrNode != null)
                    {
                        vrNode.Values.Remove(removedValue);
                    }
                    break;
                case ZWNotification.Type.ValueChanged:
                    var changedValue = notification.GetValueID();
                    Log.Debug("Node {0} Value Changed: {1} - {2} - {3}", notification.GetNodeId(),
                        manager.GetValueLabel(changedValue), GetValue(changedValue, manager),
                        manager.GetValueUnits(changedValue));
                    break;

                case ZWNotification.Type.NodeNew:
                    // if the node is not in the z-wave config this will be called first
                    var newNode = new Node
                    {
                        Id = notification.GetNodeId(),
                        HomeId = notification.GetHomeId()
                    };
                    Log.Debug("Node New: {0}, Home: {1}", newNode.Id, newNode.HomeId);
                    _nodes.Add(newNode);
                    break;
                case ZWNotification.Type.NodeAdded:
                    // if the node is in the z-wave config then this will be the first node notification
                    if (GetNode(notification.GetHomeId(), notification.GetNodeId()) == null)
                    {
                        var node = new Node
                        {
                            Id = notification.GetNodeId(),
                            HomeId = notification.GetHomeId()
                        };
                        Log.Debug("Node Added: {0}, Home: {1}", node.Id, node.HomeId);
                        _nodes.Add(node);
                    }
                    break;

                case ZWNotification.Type.NodeNaming:
                    var namedNode = GetNode(notification.GetHomeId(), notification.GetNodeId());
                    if (namedNode != null)
                    {
                        namedNode.Name = manager.GetNodeName(namedNode.HomeId, namedNode.Id);
                        namedNode.Manufacturer = manager.GetNodeManufacturerName(namedNode.HomeId, namedNode.Id);
                        namedNode.Product = manager.GetNodeProductName(namedNode.HomeId, namedNode.Id);
                        namedNode.Location = manager.GetNodeLocation(namedNode.HomeId, namedNode.Id);

                        Log.Debug("Name: {0}, Manufacturer: {1}, Product: {2}, Location: {3}", namedNode.Name,
                            namedNode.Manufacturer, namedNode.Product, namedNode.Location);
                    }
                    break;
                default:
                    Log.Trace("ZWave Notification: {0}", notificationType.ToString());
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