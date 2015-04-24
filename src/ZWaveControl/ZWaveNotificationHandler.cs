using System.Linq;
using NLog;
using OpenZWaveDotNet;

namespace ZWaveControl
{
    public class ZWaveNotificationHandler
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IZWaveNodeList _nodes;
        private readonly ZWManager _manager;

        public ZWaveNotificationHandler(IZWaveNodeList nodes, ZWManager manager)
        {
            _nodes = nodes;
            _manager = manager;
        }

        public void HandleNotification(ZWNotification notification)
        {
            var notificationType = notification.GetType();

            switch (notificationType)
            {
                case ZWNotification.Type.ValueAdded:
                    var addedValue = notification.GetValueID();
                    Log.Debug("Node {0} Value Added: {1} - {2} - {3}", notification.GetNodeId(),
                        _manager.GetValueLabel(addedValue), GetValue(addedValue, _manager),
                        _manager.GetValueUnits(addedValue));
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
                    //TODO:  This is only logging.  Remove this for simplicity
                    var changedValue = notification.GetValueID();
                    Log.Debug("Node {0} Value Changed: {1} - {2} - {3}", notification.GetNodeId(),
                        _manager.GetValueLabel(changedValue), GetValue(changedValue, _manager),
                        _manager.GetValueUnits(changedValue));
                    break;

                case ZWNotification.Type.NodeNew:
                    // if the node is not in the z-wave config this will be called first
                    var newNode = AddNode(notification.GetHomeId(), notification.GetNodeId());
                    Log.Debug("Node New: {0}, Home: {1}", newNode.NodeId, newNode.HomeId);
                    break;
                case ZWNotification.Type.NodeAdded:
                    // if the node is in the z-wave config then this will be the first node notification
                    var homeId = notification.GetHomeId();
                    var nodeId = notification.GetNodeId();
                    if (GetNode(homeId, nodeId) == null)
                    {
                        var node = AddNode(homeId, nodeId);
                        Log.Debug("Node Added: {0}, Home: {1}", node.NodeId, node.HomeId);
                    }
                    break;

                case ZWNotification.Type.NodeNaming:
                    var namedNode = GetNode(notification.GetHomeId(), notification.GetNodeId());
                    if (namedNode != null)
                    {
                        namedNode.Name = _manager.GetNodeName(namedNode.HomeId, namedNode.NodeId);
                        namedNode.Manufacturer = _manager.GetNodeManufacturerName(namedNode.HomeId, namedNode.NodeId);
                        namedNode.Product = _manager.GetNodeProductName(namedNode.HomeId, namedNode.NodeId);
                        namedNode.Location = _manager.GetNodeLocation(namedNode.HomeId, namedNode.NodeId);

                        Log.Debug("Name: {0}, Manufacturer: {1}, Product: {2}, Location: {3}", namedNode.Name,
                            namedNode.Manufacturer, namedNode.Product, namedNode.Location);
                    }
                    break;
                default:
                    Log.Trace("Unhandled ZWave Notification: {0}", notificationType.ToString());
                    break;
            }
        }

        private Node AddNode(uint homeId, byte nodeId)
        {
            var node = new Node
            {
                NodeId = nodeId,
                HomeId = homeId
            };
            _nodes.AddNode(node);
            return node;
        }

        private Node GetNode(uint homeId, byte nodeId)
        {
            return _nodes.AllNodes.FirstOrDefault(n => n.NodeId == nodeId && n.HomeId == homeId);
        }


        private static string GetValue(ZWValueID value, ZWManager manager)
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