﻿/*
 * Copyright (c) 2018 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Tizen.TV.UIControls.Forms
{
    public class OverlayMediaView : MediaView, IOverlayOutput
    {
        internal static readonly BindablePropertyKey OverlayAreaPropertyKey = BindableProperty.CreateReadOnly(nameof(OverlayArea), typeof(Rectangle), typeof(OverlayMediaView), default(Rectangle));
        public static readonly BindableProperty OverlayAreaProperty = OverlayAreaPropertyKey.BindableProperty;

        public OverlayMediaView()
        {
            BatchCommitted += OnBatchCommitted;
        }
        
        public event EventHandler AreaUpdated;

        public Rectangle OverlayArea
        {
            get { return (Rectangle)GetValue(OverlayAreaProperty); }
            private set { SetValue(OverlayAreaPropertyKey, value); }
        }

        public override VideoOuputType OuputType => VideoOuputType.Overlay;


        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (new List<string> { nameof(X), nameof(Y), nameof(Width), nameof(Height)}.Contains(propertyName) && !Batched)
            {
                OverlayArea = Bounds;
            }
            if (propertyName == nameof(OverlayArea))
            {
                AreaUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        void OnBatchCommitted(object sender, Xamarin.Forms.Internals.EventArg<VisualElement> e)
        {
            OverlayArea = Bounds;
        }
    }
}
