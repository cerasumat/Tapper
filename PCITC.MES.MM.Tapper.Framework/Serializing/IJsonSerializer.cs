﻿using System;

namespace PCITC.MES.MM.Tapper.Framework.Serializing
{
    public interface IJsonSerializer
    {
        string Serialize(object obj);
        object Deserialize(string value, Type type);
        T Deserialize<T>(string value) where T : class;
    }
}
