/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System;

namespace EnjoyGameClub.TextLifeFramework.Core.SerializedObject
{
    public interface IScriptableObjectSaveable
    {
        public Object Save(bool isRootAsset = false, Object rootObject = null, string rootPath = "");
    }

    public interface IScriptableObjectCloneable
    {
        public object Clone();
    }
}