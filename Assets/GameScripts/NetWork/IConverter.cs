using System;
using System.Collections.Generic;

public interface IConverter
{
    string  serializeObject         (object data);
    T       deserializeObject<T>    (string value);
    object  deserializeObject       (string value, Type type);
    Dictionary<string, string> deserializeObject(string value);
}
