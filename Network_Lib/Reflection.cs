using System;
using System.Collections.Generic;
using System.Reflection;

namespace Net
{
    public class Reflection
    {
        BindingFlags bindingFlags;
        Assembly executeAssembly;

        public Action<string> consoleDebugger;

        public Reflection()
        {
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
                Inspect(netObj.GetType(), netObj);
            }
        }

        public void Inspect(Type type, object obj)
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
                            ReadValue(info, obj, (NetVariable)attribute);
                        }
                    }

                    if (type.BaseType != null)
                    {
                        Inspect(type.BaseType, obj);

                    }
                }
            }
        }

        public void ReadValue(FieldInfo info, object obj, NetVariable attribute)
        {
            if (info.FieldType.IsValueType || info.FieldType == typeof(string) || info.FieldType.IsEnum)
            {
                consoleDebugger?.Invoke(info.Name + ": " + info.GetValue(obj));
                SendPackage(info, obj, attribute);
            }
            else if (typeof(System.Collections.ICollection).IsAssignableFrom(info.FieldType))
            {
                foreach (object item in (info.GetValue(obj) as System.Collections.ICollection))
                {
                    Inspect(item.GetType(), item);
                }
            }
            else
            {
                consoleDebugger?.Invoke("Inspect: " + info.Name);
                Inspect(info.FieldType, info.GetValue(obj));
            }
        }

        public void SendPackage(FieldInfo info, object obj, NetVariable attribute)
        {
            Type packageType = info.GetValue(obj).GetType();  //Por reflection hay qe obtener todos los tipos de mensajes y creo el tipo de mensaje que coincida con getType

            foreach (Type type in executeAssembly.GetTypes())
            {
                if (type.IsClass && type.BaseType != null && type.BaseType.IsGenericType)
                {
                    Type[] genericTypes = type.BaseType.GetGenericArguments();

                    foreach (Type arg in genericTypes)
                    {
                        if (packageType == arg)
                        {
                            //Create message
                            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

                            Type[] parametersToApply =
                                { typeof(MessagePriority), packageType };

                            object[] parameters = new[] { attribute.MessagePriority, info.GetValue(obj) };

                            ConstructorInfo? ctor = type.GetConstructor(parametersToApply);

                            consoleDebugger.Invoke("Contructor: " + ctor);

                            if (ctor != null)
                            {
                                object message = ctor.Invoke(parameters);
                                var a = (message as ParentBaseMessage);

                                consoleDebugger.Invoke(a.Test());
                            }
                        }
                    }
                }
            }
        }

        public void WriteInspect(Type type, object obj, byte[] data) //Type deberia ser siempre un NetObj y el obj lo saco de la lista del factory
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
                            WriteValue(info, obj);
                        }
                    }

                    if (type.BaseType != null)
                    {
                        Inspect(type.BaseType, obj);
                    }
                }
            }
        }

        public void WriteValue(FieldInfo info, object obj)
        {
            if (info.FieldType.IsValueType || info.FieldType == typeof(string) || info.FieldType.IsEnum)
            {
                Console.WriteLine(info.Name + ": " + info.GetValue(obj));
            }
            else if (typeof(System.Collections.ICollection).IsAssignableFrom(info.FieldType))
            {
                foreach (object item in (info.GetValue(obj) as System.Collections.ICollection))
                {
                    Inspect(item.GetType(), item);
                }
            }
            else
            {
                Inspect(info.FieldType, info.GetValue(obj));
            }
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

