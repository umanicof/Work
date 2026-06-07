using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

namespace NkfLib.Unity
{
    // 出典：https://qiita.com/Zuishin/items/61fc8807d027d5cea329
    // ジェネリックの四則演算・比較に使う。作っては見たものの、ジェネリック引数の組み合わせが増えると記述も煩雑になり余りよろしくない
#if false
    public class Operations<T, U>
    {
        static ParameterExpression _p1 = Expression.Parameter(typeof(T));
        static ParameterExpression _p2 = Expression.Parameter(typeof(U));

        public static Func<T, U, T> Add { get; } = Expression.Lambda<Func<T, U, T>>(Expression.Add(_p1, _p2), _p1, _p2).Compile();
        public static Func<T, U, T> Subtract { get; } = Expression.Lambda<Func<T, U, T>>(Expression.Subtract(_p1, _p2), _p1, _p2).Compile();
        public static Func<T, U, T> Multiply { get; } = Expression.Lambda<Func<T, U, T>>(Expression.Multiply(_p1, _p2), _p1, _p2).Compile();
        public static Func<T, U, T> Divide { get; } = Expression.Lambda<Func<T, U, T>>(Expression.Divide(_p1, _p2), _p1, _p2).Compile();
        public static Func<T, U, T> Modulo { get; } = Expression.Lambda<Func<T, U, T>>(Expression.Modulo(_p1, _p2), _p1, _p2).Compile();
        public static Func<T, U, bool> Equal { get; } = Expression.Lambda<Func<T, U, bool>>(Expression.Equal(_p1, _p2), _p1, _p2).Compile();
        public static Func<T, U, bool> GreaterThan { get;} = Expression.Lambda<Func<T, U, bool>>(Expression.GreaterThan(_p1, _p2), _p1, _p2).Compile();
        public static Func<T, U, bool> GreaterThanOrEqual { get; } = Expression.Lambda<Func<T, U, bool>>(Expression.GreaterThanOrEqual(_p1, _p2), _p1, _p2).Compile();
        public static Func<T, U, bool> LessThan { get; } = Expression.Lambda<Func<T, U, bool>>(Expression.LessThan(_p1, _p2), _p1, _p2).Compile();
        public static Func<T, U, bool> LessThanOrEqual { get;  } = Expression.Lambda<Func<T, U, bool>>(Expression.LessThanOrEqual(_p1, _p2), _p1, _p2).Compile();
    }
#endif

#if true
    // こちらの方がまだマシか。ただし処理負荷はやや高めのはず。まだdynamicを使った方が良いのかも知れない。
    public class Operations
    {
        public static T Add<T, U>(T lhs, U rhs)
        {
            var p1 = Expression.Parameter(typeof(T));
            var p2 = Expression.Parameter(typeof(U));
            var lamda = Expression.Lambda<Func<T, U, T>>(Expression.Add(p1, p2), p1, p2).Compile();
            return lamda(lhs, rhs);
        }
        public static T Subtract<T, U>(T lhs, U rhs)
        {
            var p1 = Expression.Parameter(typeof(T));
            var p2 = Expression.Parameter(typeof(U));
            var lamda = Expression.Lambda<Func<T, U, T>>(Expression.Subtract(p1, p2), p1, p2).Compile();
            return lamda(lhs, rhs);
        }
        public static T Multiply<T, U>(T lhs, U rhs)
        {
            var p1 = Expression.Parameter(typeof(T));
            var p2 = Expression.Parameter(typeof(U));
            var lamda = Expression.Lambda<Func<T, U, T>>(Expression.Multiply(p1, p2), p1, p2).Compile();
            return lamda(lhs, rhs);
        }
        public static T Divide<T, U>(T lhs, U rhs)
        {
            var p1 = Expression.Parameter(typeof(T));
            var p2 = Expression.Parameter(typeof(U));
            var lamda = Expression.Lambda<Func<T, U, T>>(Expression.Divide(p1, p2), p1, p2).Compile();
            return lamda(lhs, rhs);
        }
        public static bool Equal<T, U>(T lhs, U rhs)
        {
            var p1 = Expression.Parameter(typeof(T));
            var p2 = Expression.Parameter(typeof(U));
            var lamda = Expression.Lambda<Func<T, U, bool>>(Expression.Equal(p1, p2), p1, p2).Compile();
            return lamda(lhs, rhs);
        }
        public static bool GreaterThan<T, U>(T lhs, U rhs)
        {
            var p1 = Expression.Parameter(typeof(T));
            var p2 = Expression.Parameter(typeof(U));
            var lamda = Expression.Lambda<Func<T, U, bool>>(Expression.GreaterThan(p1, p2), p1, p2).Compile();
            return lamda(lhs, rhs);
        }
        public static bool GreaterThanOrEqual<T, U>(T lhs, U rhs)
        {
            var p1 = Expression.Parameter(typeof(T));
            var p2 = Expression.Parameter(typeof(U));
            var lamda = Expression.Lambda<Func<T, U, bool>>(Expression.GreaterThanOrEqual(p1, p2), p1, p2).Compile();
            return lamda(lhs, rhs);
        }
        public static bool LessThan<T, U>(T lhs, U rhs)
        {
            var p1 = Expression.Parameter(typeof(T));
            var p2 = Expression.Parameter(typeof(U));
            var lamda = Expression.Lambda<Func<T, U, bool>>(Expression.LessThan(p1, p2), p1, p2).Compile();
            return lamda(lhs, rhs);
        }
        public static bool LessThanOrEqual<T, U>(T lhs, U rhs)
        {
            var p1 = Expression.Parameter(typeof(T));
            var p2 = Expression.Parameter(typeof(U));
            var lamda = Expression.Lambda<Func<T, U, bool>>(Expression.LessThanOrEqual(p1, p2), p1, p2).Compile();
            return lamda(lhs, rhs);
        }
    }
#endif
}