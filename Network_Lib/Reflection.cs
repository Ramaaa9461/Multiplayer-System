using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Net
{
    public class Reflection
    {
        BindingFlags bindingFlags;
        Assembly executeAssembly;

        public Action<string> consoleDebugger;
        public Action consoleDebuggerPause;

        NetworkEntity networkEntity;

        public Reflection(NetworkEntity entity)
        {
            networkEntity = entity;
            networkEntity.OnReceivedMessage += OnReceivedReflectionMessage;

            executeAssembly = Assembly.GetExecutingAssembly();

            bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        }

        public void UpdateReflection()
        {
            if (NetObjFactory.NetObjectsCount <= 0)
            {
                return;
            }

            foreach (INetObj netObj in NetObjFactory.NetObjects)
            {
                if (netObj.GetOwnerID() == networkEntity.clientID)
                {
                    List<int> idRoute = new List<int>();
                    idRoute.Add(netObj.GetID());
                    Inspect(netObj.GetType(), netObj, idRoute);
                }
            }
        }

        public void Inspect(Type type, object obj, List<int> idRoute)
        {

            if (obj != null)
            {
                foreach (FieldInfo info in type.GetFields(bindingFlags))
                {
                    IEnumerable<Attribute> attributes = info.GetCustomAttributes();

                    foreach (Attribute attribute in attributes)
                    {
                        if (attribute is NetVariable)
                        {


                            ReadValue(info, obj, (NetVariable)attribute, new List<int>(idRoute));
                        }
                    }

                    if (type.BaseType != null)
                    {
                        Inspect(type.BaseType, obj, new List<int>(idRoute));
                    }
                }
            }
        }

        public void ReadValue(FieldInfo info, object obj, NetVariable attribute, List<int> idRoute)
        {
            if (info.FieldType.IsValueType || info.FieldType == typeof(string) || info.FieldType.IsEnum)
            {
                idRoute.Add(attribute.VariableId);

                string debug = "";
                debug += "Read values from Root Player (Owner: " + NetObjFactory.GetINetObject(idRoute[0]).GetOwnerID().ToString() + ") \n";
                debug += "Se modifica la variable " + info + " que tiene un valor de " + info.GetValue(obj) + "\n";

                debug += "La ruta de la variable es: ";
                foreach (int item in idRoute)
                {
                    debug += item + " - ";
                }

                //consoleDebugger.Invoke(debug);

                SendPackage(info, obj, attribute, idRoute);
            }
            else if (typeof(System.Collections.ICollection).IsAssignableFrom(info.FieldType))
            {
                foreach (object item in (info.GetValue(obj) as System.Collections.ICollection))
                {
                    Inspect(item.GetType(), item, idRoute); //TODO: ver qe onda las colleciones, tiene qe agregarse a idRoute
                }
            }
            else
            {
                idRoute.Add(attribute.VariableId);
                Inspect(info.FieldType, info.GetValue(obj), idRoute);
            }
        }

        public void SendPackage(FieldInfo info, object obj, NetVariable attribute, List<int> idRoute)
        {
            Type packageType = info.GetValue(obj).GetType();  //Por reflection hay qe obtener todos los tipos de mensajes y creo el tipo de mensaje que coincida con getType

            foreach (Type type in executeAssembly.GetTypes())
            {
                if (type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(BaseMessage<>))
                {
                    Type[] genericTypes = type.BaseType.GetGenericArguments();

                    foreach (Type arg in genericTypes)
                    {
                        if (packageType == arg)
                        {
                            Type[] parametersToApply = { typeof(MessagePriority), packageType, typeof(List<int>) };

                            object[] parameters = new[] { attribute.MessagePriority, info.GetValue(obj), idRoute };

                            ConstructorInfo? ctor = type.GetConstructor(parametersToApply);

                            if (ctor != null)
                            {
                                object message = ctor.Invoke(parameters);
                                ParentBaseMessage a = message as ParentBaseMessage;
                                networkEntity.SendMessage(a.Serialize());

                                string debug = "";
                                debug += "SEND PACKAGE Root Player (Owner: " + NetObjFactory.GetINetObject(idRoute[0]).GetOwnerID().ToString() + ") \n";
                                debug += "Se modifica la variable " + info + " que tiene un valor de " + info.GetValue(obj) + "\n";
                                debug += "El tipo de mensajes " + MessageChecker.CheckMessageType(a.Serialize()) + "\n";
                                debug += "Al constructor se le paso: " + attribute.MessagePriority + " - " + info.GetValue(obj) + " - La ruta " + "\n";

                                debug += "La ruta de la variable es: ";
                                foreach (int item in idRoute)
                                {
                                    debug += item + " - ";
                                }

                                //consoleDebugger.Invoke(debug);
                            }
                        }
                    }
                }
            }
        }

        public void OnReceivedReflectionMessage(byte[] data, IPEndPoint ip)
        {
            // DeserializeReflectionMessage(data);

            switch (MessageChecker.CheckMessageType(data))
            {
                case MessageType.Ulong:
                    break;
                case MessageType.Uint:
                    break;
                case MessageType.Ushort:
                    break;
                case MessageType.String:
                    break;
                case MessageType.Short:
                    break;
                case MessageType.Sbyte:
                    break;
                case MessageType.Long:
                    break;
                case MessageType.Int:
                    break;
                case MessageType.Float:

                    NetFloatMessage netFloatMessage = new NetFloatMessage(data);
                    VariableMapping(netFloatMessage.GetMessageRoute(), netFloatMessage.GetData());

                    break;
                case MessageType.Double:
                    break;
                case MessageType.Decimal:
                    break;
                case MessageType.Char:
                    break;
                case MessageType.Byte:
                    break;
                case MessageType.Bool:

                    NetBoolMessage netBoolMessage = new NetBoolMessage(data);
                    VariableMapping(netBoolMessage.GetMessageRoute(), netBoolMessage.GetData());

                    break;
            }
        }

        void DeserializeReflectionMessage(byte[] data)
        {
            MessageType messageType = MessageChecker.CheckMessageType(data);  //Por reflection hay qe obtener todos los tipos de mensajes y creo el tipo de mensaje que coincida con getType

            foreach (Type type in executeAssembly.GetTypes())
            {
                if (type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(BaseMessage<>))
                {
                    NetMessageClass attribute = type.GetCustomAttribute<NetMessageClass>();

                    if (attribute.MessageType == messageType)
                    {
                        Type[] parametersToApply = { typeof(byte[]) };

                        object[] parameters = new[] { data };

                        ConstructorInfo? ctor = attribute.GetType().GetConstructor(parametersToApply); //Attribute.GetType me da la clase que necesito,

                        if (ctor != null)
                        {
                            object message = ctor.Invoke(parameters);
                            //ParentBaseMessage a = message as ParentBaseMessage;

                            consoleDebugger.Invoke("Se creo el Message " + message.ToString());
                            CastToCorrectMessage(message, data);
                        }
                    }
                }
            }
        }

        void CastToCorrectMessage(object obj, byte[] data)
        {
            if (obj.GetType() == typeof(NetFloatMessage))
            {
                consoleDebugger.Invoke("Se casteo " + obj.GetType());

                NetFloatMessage netFloatMessage = new NetFloatMessage(data);
                VariableMapping(netFloatMessage.GetMessageRoute(), netFloatMessage.GetData());
            }
        }

        void VariableMapping(List<int> route, object variableValue)
        {
            if (route.Count > 0)
            {
                INetObj objectRoot = NetObjFactory.GetINetObject(route[0]);

                if (objectRoot.GetOwnerID() != networkEntity.clientID)
                {
                    InspectWrite(objectRoot.GetType(), objectRoot, route, 1, variableValue);
                }
            }
        }

        public void InspectWrite(Type type, object obj, List<int> idRoute, int idToRead, object value)
        {
            if (obj != null)
            {
                foreach (FieldInfo info in type.GetFields(bindingFlags))
                {
                    IEnumerable<Attribute> attributes = info.GetCustomAttributes();
                    foreach (Attribute attribute in attributes)
                    {
                        if (attribute is NetVariable && ((NetVariable)attribute).VariableId == idRoute[idToRead])
                        {
                            WriteValue(info, obj, (NetVariable)attribute, idRoute, idToRead, value);
                            break;
                        }
                    }

                    ///if (type.BaseType != null)
                    ///{
                    ///    InspectWrite(type.BaseType, obj, idRoute, idToRead++, value);
                    ///}
                }
            }
        }

        void WriteValue(FieldInfo info, object obj, NetVariable attribute, List<int> idRoute, int idToRead, object value)
        {
            if (info.FieldType.IsValueType || info.FieldType == typeof(string) || info.FieldType.IsEnum)
            {
                string debug = "";
                debug += "Write values from Root Player (Owner: " + NetObjFactory.GetINetObject(idRoute[0]).GetOwnerID().ToString() + ") \n";
                debug += "Se modifica la variable " + info + " que tiene un valor de " + info.GetValue(obj) + ". El nuevo valor a asignar es: " + value + "\n";

                debug += "La ruta de la variable es: ";
                foreach (int item in idRoute)
                {
                    debug += item + " - ";
                }

                consoleDebugger.Invoke(debug);
                info.SetValue(obj, value);
            }
            else if (typeof(System.Collections.ICollection).IsAssignableFrom(info.FieldType))
            {
                foreach (object item in (info.GetValue(obj) as System.Collections.ICollection))
                {
                    InspectWrite(item.GetType(), item, idRoute, idToRead, value);
                }
            }
            else
            {
                idToRead++;
                InspectWrite(info.FieldType, info.GetValue(obj), idRoute, idToRead, value);
            }
        }

    }

    public class NetMessageClass : Attribute
    {
        Type type;
        MessageType messageType;

        public NetMessageClass(Type type, MessageType messageType)
        {
            this.type = type;
            this.messageType = messageType;
        }
        public MessageType MessageType
        {
            get { return messageType; }
        }

        public Type Type
        {
            get { return type; }
        }
    }

    public class NetVariable : Attribute
    {
        int variableId;
        MessagePriority messagePriority;

        public NetVariable(int id, MessagePriority messagePriority = MessagePriority.Default)
        {
            variableId = id;
            this.messagePriority = messagePriority;
        }

        public MessagePriority MessagePriority
        {
            get { return messagePriority; }
        }

        public int VariableId
        {
            get { return variableId; }
        }
    }
}

