using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NkfLib.Unity
{
    public static class EnumExtensions
    {
        // <summary>
        /// enum => ビット変換
        /// ・値が32以上だと例外発生
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int ToBit(this Enum self)
        {
            int value = Convert.ToInt32(self);
            if (value >= 32) throw new ArgumentException();
            return 0x01 << value;
        }

        /// <summary>
        /// ビット => enum変換
        /// ・ビットがONされていなければ例外発生
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static T BitToEnum<T>(this int self) where T : Enum
        {
            for (int i = 0; i < 32; ++i)
            {
                if ((self & (0x01 << i)) != 0)
                {
                    return (T)(object)i;
                }
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// ビット => enumリスト変換
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static List<T> BitToEnumAll<T>(this int self) where T : Enum
        {
            var list = new List<T>();
            for (int i = 0; i < 32; ++i)
            {
                if ((self & (0x01 << i)) != 0)
                {
                    list.Add((T)(object)i);
                }
            }
            return list;
        }
    }
}