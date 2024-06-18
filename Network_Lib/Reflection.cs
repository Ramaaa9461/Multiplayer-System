using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Net
{
    public class Reflection
    {
        BindingFlags bindingFlags;

        void Initialize() //Necesito una referencia al Factory de NetObj
        {
            bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
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
                            ReadValue(info, obj, attribute);
                        }
                    }

                    if (type.BaseType != null)
                    {
                        Inspect(type.BaseType, obj);
                    }
                }
            }
        }

        public void ReadValue(FieldInfo info, object obj, Attribute attribute)
        {
            if (info.FieldType.IsValueType || info.FieldType == typeof(string) || info.FieldType.IsEnum)
            {
                Console.WriteLine(info.Name + ": " + info.GetValue(obj));
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
                Inspect(info.FieldType, info.GetValue(obj));
            }
        }

        public void SendPackage(FieldInfo info, object obj, Attribute attribute)
        {
            object packageObj = info.GetValue(obj);

            packageObj.GetType();  //Por reflection hay qe obtener todos los tipos de mensajes y creo el tipo de mensaje que coincida con getType

            //TODO: Falta el SendMessage, necesito una refe de networkEntity   
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

