using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Net;

public class Reflection : MonoBehaviour
{
    GameManager gm;
    BindingFlags bindingFlags;

    [SerializeField] PlayerController pc;

    void Start()
    {
        gm = GameManager.Instance;

        //        Inspect(typeof(PlayerController), gm.playerList[0]);

        // foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        // {
        // }
        bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Inspect(typeof(PlayerController), pc);
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
                        ReadValue(info, obj);
                    }
                }

                if (type.BaseType != null)
                {
                    Inspect(type.BaseType, obj);
                }
            }
        }
    }

    public void ReadValue(FieldInfo info, object obj)
    {
        if (info.FieldType.IsValueType || info.FieldType == typeof(string) || info.FieldType.IsEnum)
        {
            Debug.Log(info.Name + ": " + info.GetValue(obj));
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

    public void SendPackage(FieldInfo info, object obj)
    {
        object packageObj = info.GetValue(obj);

        if (packageObj is int intValue)
        {
            Debug.Log($"Is int: {intValue}");
        }
        else if (packageObj is float floatValue)
        {
            Debug.Log($"Is float: {floatValue}");
        }
        else if (packageObj is double doubleValue)
        {
            Debug.Log($"Is double: {doubleValue}");
        }
        else if (packageObj is bool boolValue)
        {
            Debug.Log($"Is bool: {boolValue}");
        }
        else if (packageObj is string stringValue)
        {
            Debug.Log($"Is string: {stringValue}");
        }
        else if (packageObj is char charValue)
        {
            Debug.Log($"Is char: {charValue}");
        }
        else if (packageObj is byte byteValue)
        {
            Debug.Log($"Is byte: {byteValue}");
        }
        else if (packageObj is sbyte sbyteValue)
        {
            Debug.Log($"Is sbyte: {sbyteValue}");
        }
        else if (packageObj is short shortValue)
        {
            Debug.Log($"Is short: {shortValue}");
        }
        else if (packageObj is long longValue)
        {
            Debug.Log($"Is long: {longValue}");
        }
        else if (packageObj is uint uintValue)
        {
            Debug.Log($"Is uint: {uintValue}");
        }
        else if (packageObj is ushort ushortValue)
        {
            Debug.Log($"Is ushort: {ushortValue}");
        }
        else if (packageObj is ulong ulongValue)
        {
            Debug.Log($"Is ulong: {ulongValue}");
        }
        else if (packageObj is decimal decimalValue)
        {
            Debug.Log($"Is decimal: {decimalValue}");
        }
        else
        {
            Debug.Log("Unknown type");
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
                        Debug.Log($"NetVariable with VariableId: {((NetVariable)attribute).VariableId}");
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
            Debug.Log(info.Name + ": " + info.GetValue(obj));
            SendPackage(info, obj);
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

    public int VariableId
    {
        get { return variableId; }
    }
}