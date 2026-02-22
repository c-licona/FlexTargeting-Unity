// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cyclic.FlexTargeting
{
    /// <summary>
    /// Encapsulates an object of type <typeparamref name="T"/> into a special adapter object that implements
    /// <see cref="IFlexTarget"/> so that the object can be searched for in the <see cref="FlexTargetingCore"/>
    /// methods. In order for this adaption to work, the static functions that are used to implement the
    /// <see cref="IFlexTarget"/> interface must first be defined before this adapter class of type
    /// <typeparamref name="T"/> is used in any <see cref="FlexTargetingCore"/> methods. These functions can be set
    /// from <see cref="FlexTargetAdapter.SetAdapterFuncs"/> or <see cref="FlexTargetAdapter.Adapt"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object that will be adapted</typeparam>
    public class FlexTargetAdapter<T> : IFlexTarget
    {
        /// <summary>
        /// The encapsulated target of type <typeparamref name="T"/>
        /// </summary>
        public T Target { get; set; }

        public bool IsTargetValid => IsTargetValidFunc.Invoke(Target);
        public Vector3 TargetPosition => TargetPositionFunc.Invoke(Target);
        public float LosBufferRadius => LosBufferRadiusFunc.Invoke(Target);

        public FlexTargetAdapter(T target) => Target = target;

        // the functions will throw exceptions by default until they are properly implemented
        internal static Func<T, bool> IsTargetValidFunc { get; set; } = _ => throw new NotImplementedException();
        internal static Func<T, Vector3> TargetPositionFunc { get; set; } = _ => throw new NotImplementedException();
        internal static Func<T, float> LosBufferRadiusFunc { get; set; } = _ => throw new NotImplementedException();
    }

    /// <summary>
    /// Contains methods for adapting objects into flex targets so that they can be searched for using the
    /// <see cref="FlexTargetingCore"/> methods without having to explicitly have those object types implement
    /// <see cref="IFlexTarget"/> themselves.
    /// </summary>
    public static class FlexTargetAdapter
    {
        /// <summary>
        /// Set the required adapter functions for type <typeparamref name="T"/>. Once these functions are set, this
        /// type can be adapted with <see cref="FlexTargetAdapter{T}"/>s using the <see cref="AdaptWithoutFuncs"/>
        /// method, and then the resulting list of adapted targets can be used in the <see cref="FlexTargetingCore"/>
        /// methods. It may be more convenient to use the <see cref="Adapt{T}"/> method which will adapt a list of
        /// targets of type <typeparamref name="T"/> AND set the functions all in one go.
        /// </summary>
        /// <param name="isTargetValidFunc">This function implements the <see cref="IFlexTarget.IsTargetValid"/>
        /// interface method. See its documentation for more info.</param>
        /// <param name="targetPositionFunc">This function implements the <see cref="IFlexTarget.TargetPosition"/>
        /// interface method. See its documentation for more info.</param>
        /// <param name="losBufferRadiusFunc">This function implements the <see cref="IFlexTarget.LosBufferRadius"/>
        /// interface method. See its documentation for more info.</param>
        /// <typeparam name="T">The type of the object that will be adapted.</typeparam>
        public static void SetAdapterFuncs<T>(Func<T, bool> isTargetValidFunc,
            Func<T, Vector3> targetPositionFunc,
            Func<T, float> losBufferRadiusFunc)
        {
            FlexTargetAdapter<T>.IsTargetValidFunc = isTargetValidFunc;
            FlexTargetAdapter<T>.TargetPositionFunc = targetPositionFunc;
            FlexTargetAdapter<T>.LosBufferRadiusFunc = losBufferRadiusFunc;
        }

        /// <summary>
        /// <para>
        /// Adapts the list of objects of type <typeparamref name="T"/> into Flex Targets. This list can then be used
        /// in the <see cref="FlexTargetingCore"/> methods for target finding.
        /// </para>
        /// <para>
        /// To perform this adaption, the 3 functions in the parameter list must be defined. These functions will be
        /// used in the <see cref="FlexTargetAdapter{T}"/> to satisfy the <see cref="IFlexTarget"/> interface. This
        /// will allow the adapted object of type <typeparamref name="T"/> to be used in the
        /// <see cref="FlexTargetingCore"/> methods.
        /// </para>
        /// </summary>
        /// <param name="listToAdapt">The list of objects to adapt into Flex Targets</param>
        /// <param name="isTargetValidFunc">This function implements the <see cref="IFlexTarget.IsTargetValid"/>
        /// interface method. See its documentation for more info.</param>
        /// <param name="targetPositionFunc">This function implements the <see cref="IFlexTarget.TargetPosition"/>
        /// interface method. See its documentation for more info.</param>
        /// <param name="losBufferRadiusFunc">This function implements the <see cref="IFlexTarget.LosBufferRadius"/>
        /// interface method. See its documentation for more info.</param>
        /// <typeparam name="T">The type of the object that is being adapted.</typeparam>
        /// <returns>Returns a NEW list of <see cref="FlexTargetAdapter{T}"/>s that adapt the given object type.</returns>
        public static IReadOnlyList<FlexTargetAdapter<T>> Adapt<T>(this IReadOnlyList<T> listToAdapt,
            Func<T, bool> isTargetValidFunc,
            Func<T, Vector3> targetPositionFunc,
            Func<T, float> losBufferRadiusFunc)
        {
            SetAdapterFuncs(isTargetValidFunc, targetPositionFunc, losBufferRadiusFunc);
            return listToAdapt.AdaptWithoutFuncs();
        }

        /// <summary>
        /// Adapt the list of objects of type <typeparamref name="T"/> into Flex Targets without providing functions
        /// that are required for the adaption.
        /// This should ONLY be used if the functions have already been set with <see cref="SetAdapterFuncs{T}"/>.
        /// Alternatively, use the <see cref="Adapt{T}"/> method which is generally safer since the signature forces you
        /// to set the functions along with adapting the list.
        /// </summary>
        /// <param name="listToAdapt">The list of objects to adapt into Flex Targets</param>
        /// <typeparam name="T">The type of the object that will be adapted</typeparam>
        /// <returns>Returns a NEW list of <see cref="FlexTargetAdapter{T}"/>s that adapt the given object type.</returns>
        /// <exception cref="NotImplementedException">An exception will be thrown, not by this method, but if the
        /// adapted list is used in a target finding method without first setting the required adapter functions. Set
        /// these functions either with <see cref="SetAdapterFuncs{T}"/> or with the safer <see cref="Adapt{T}"/>
        /// method which forces you to set the functions along with adapting the list.
        /// </exception>
        public static IReadOnlyList<FlexTargetAdapter<T>> AdaptWithoutFuncs<T>(this IReadOnlyList<T> listToAdapt)
        {
            var adaptedList = new List<FlexTargetAdapter<T>>(listToAdapt.Count);

            for (int i = 0; i < listToAdapt.Count; i++)
            {
                adaptedList.Add( new FlexTargetAdapter<T>(listToAdapt[i]) );
            }

            return adaptedList;
        }
    }
}
