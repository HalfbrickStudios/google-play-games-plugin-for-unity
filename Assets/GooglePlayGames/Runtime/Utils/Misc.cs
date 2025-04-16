// <copyright file="Misc.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

using System;

namespace GooglePlayGames.Utils {

    internal static class Misc {

        public static bool AreBuffersIdentical(byte[] a, byte[] b) {
            if (a == b) return true;
            if (a == null || b == null) return false;
            if (a.Length != b.Length  ) return false;
            for (var i = 0; i < a.Length; i += 1) {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        public static T CheckNotNull<T>(T value)
        {
            if (value == null) {
                Logger.e($"An argument is null when it must not");
                throw new ArgumentNullException();
            }
            return value;
        }

        public static T CheckNotNull<T>(T value, string name)
        {
            if (value == null) {
                Logger.e($"Argument \"{name}\" is null when it must not");
                throw new ArgumentNullException(name);
            }
            return value;
        }

        public static int CheckPositive(int value, string name)
        {
            if (value < 0) {
                Logger.e($"Argument \"{name}\" is negative when it must not");
                throw new ArgumentNullException(name);
            }
            return value;
        }

        public static byte[] GetSubsetBytes(byte[] array, int offset, int length)
        {
            CheckNotNull(array, nameof(array));
            if (offset < 0 || offset >= array.Length) throw new ArgumentOutOfRangeException("offset");
            if (length < 0 || (array.Length - offset) < length) throw new ArgumentOutOfRangeException("length");
            if (offset == 0 && length == array.Length) return array;
            var piece = new byte[length];
            Array.Copy(array, offset, piece, 0, length);
            return piece;
        }

    }

}