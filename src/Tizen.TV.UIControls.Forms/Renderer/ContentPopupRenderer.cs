﻿/*
 * Copyright (c) 2018 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Flora License, Version 1.1 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://floralicense.org/license/
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;
using ElmSharp;
using XForms = Xamarin.Forms.Forms;
using XColor = Xamarin.Forms.Color;
using EColor = ElmSharp.Color;

[assembly: Dependency(typeof(Tizen.TV.UIControls.Forms.Renderer.ContentPopupRenderer))]
namespace Tizen.TV.UIControls.Forms.Renderer
{
    public class ContentPopupRenderer : IContentPopupRenderer
    {
        Popup _popup;
        ContentPopup _element;
        TaskCompletionSource<bool> _tcs;

        public ContentPopupRenderer()
        {
            _popup = new Popup(XForms.NativeParent)
            {
                Style = "full",
                Orientation = PopupOrientation.Center,
            };
            _popup.BackButtonPressed += OnBackButtonPressed;
            _popup.Dismissed += OnDismissed;
        }

        ~ContentPopupRenderer()
        {
            Dispose(false);
        }

        public void SetElement(ContentPopup element)
        {
            if (element.Parent == null)
                element.Parent = Application.Current;
            element.PropertyChanged += OnElementPropertyChanged;
            _element = element;

            UpdateContent();
            if (_element.BackgroundColor != XColor.Default)
            {
                UpdateBackgroundColor();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task Open()
        {
            _popup.Show();
            _element.SetValueFromRenderer(ContentPopup.IsOpenProperty, true);
            _tcs = new TaskCompletionSource<bool>();
            return _tcs.Task;
        }

        protected void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ContentPopup.ContentProperty.PropertyName)
            {
                UpdateContent();
            }
            else if (e.PropertyName == ContentPopup.IsOpenProperty.PropertyName)
            {
                UpdateIsOpen();
            }
            else if (e.PropertyName == ContentPopup.BackgroundColorProperty.PropertyName)
            {
                UpdateBackgroundColor();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_popup != null)
                {
                    _popup.BackButtonPressed -= OnBackButtonPressed;
                    _popup.Dismissed -= OnDismissed;
                    _popup.Unrealize();
                    _popup = null;
                }
                if (_element != null)
                {
                    _element.PropertyChanged -= OnElementPropertyChanged;
                    _element = null;
                }
            }
        }

        void OnBackButtonPressed(object sender, EventArgs e)
        {
            if (!_element.SendBackButtonPressed())
                _popup?.Hide();
        }

        void OnDismissed(object sender, EventArgs e)
        {
            _element.SendDismissed();
            _tcs?.SetResult(true);
        }

        void UpdateContent()
        {
            if (_element.Content != null)
            {
                var renderer = Platform.GetOrCreateRenderer(_element.Content);
                (renderer as LayoutRenderer)?.RegisterOnLayoutUpdated();
                var native = renderer.NativeView;
                native.MinimumHeight = XForms.NativeParent.Geometry.Height;
                native.MinimumWidth = XForms.NativeParent.Geometry.Width;
                _popup.SetContent(native, false);
            }
            else
            {
                _popup.SetContent(null, false);
            }
        }

        void UpdateIsOpen()
        {
            if (!_element.IsOpen)
                _popup?.Hide();
        }

        void UpdateBackgroundColor()
        {
            var backgroundColor = _element.BackgroundColor;
            _popup.BackgroundColor = (backgroundColor == XColor.Default) ? EColor.Transparent : _element.BackgroundColor.ToNative();
        }
    }
}
