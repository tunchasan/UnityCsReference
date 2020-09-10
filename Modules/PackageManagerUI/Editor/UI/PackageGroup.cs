// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace UnityEditor.PackageManager.UI
{
    internal class PackageGroup : VisualElement
    {
        public event Action<bool> onGroupToggle = delegate {};

        public bool expanded => headerCaret.value;

        public IEnumerable<PackageItem> packageItems => groupContainer.Children().Cast<PackageItem>();

        public PackageGroup(string groupName, string displayName, bool expanded = true, bool hidden = false)
        {
            name = groupName;
            var root = Resources.GetTemplate("PackageGroup.uxml");
            Add(root);
            m_Cache = new VisualElementCache(root);

            headerCaret.SetValueWithoutNotify(expanded);
            EnableInClassList("collapsed", !expanded);
            headerCaret.RegisterValueChangedCallback((evt) =>
            {
                PageManager.instance.SetGroupExpanded(groupName, evt.newValue);
                EnableInClassList("collapsed", !evt.newValue);
                EditorApplication.delayCall += () => onGroupToggle?.Invoke(evt.newValue);
            });

            headerTag.pickingMode = PickingMode.Ignore;
            headerCaret.text = displayName;

            if (hidden)
                AddToClassList("hidden");
        }

        public bool Contains(ISelectableItem item)
        {
            return groupContainer.Contains(item.element);
        }

        public void RefreshHeaderVisibility()
        {
            EnableInClassList("empty", !packageItems.Any(item => item.visualState.visible));
        }

        internal PackageItem AddPackageItem(IPackage package, VisualState state)
        {
            var packageItem = new PackageItem(package, state) {packageGroup = this};
            groupContainer.Add(packageItem);
            return packageItem;
        }

        internal void AddPackageItem(PackageItem item)
        {
            groupContainer.Add(item);
        }

        internal void ClearPackageItems()
        {
            groupContainer.Clear();
        }

        internal void RemovePackageItem(PackageItem item)
        {
            if (groupContainer.Contains(item))
                groupContainer.Remove(item);
        }

        private readonly VisualElementCache m_Cache;

        public VisualElement groupContainer => m_Cache.Get<VisualElement>("groupContainer");
        public VisualElement headerContainer => m_Cache.Get<VisualElement>("headerContainer");
        private Toggle headerCaret => m_Cache.Get<Toggle>("headerCaret");
        private Label headerTag => m_Cache.Get<Label>("headerTag");
    }
}
