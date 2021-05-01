using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;

namespace Mvvm.Core {
    public static class Interaction {
        const string BehaviorsPropertyName = "BehaviorsInternal";
        const string BehaviorsTemplatePropertyName = "BehaviorsTemplate";
        [IgnoreDependencyPropertiesConsistencyChecker]
        public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached(BehaviorsPropertyName, typeof(AttachableCollection<Behavior>), typeof(Interaction), new PropertyMetadata(null, OnCollectionChanged));
        [IgnoreDependencyPropertiesConsistencyChecker]
        public static readonly DependencyProperty BehaviorsTemplateProperty =
            DependencyProperty.RegisterAttached(BehaviorsTemplatePropertyName, typeof(DataTemplate), typeof(Interaction), new PropertyMetadata(null, OnBehaviorsTemplateChanged));
        [IgnoreDependencyPropertiesConsistencyChecker]
        static readonly DependencyProperty BehaviorsTemplateItemsProperty = 
            DependencyProperty.RegisterAttached("BehaviorsTemplateItems", typeof(IList<Behavior>), typeof(Interaction), new PropertyMetadata(null));

        public static AttachableCollection<Behavior> GetBehaviors(DependencyObject d) {
            AttachableCollection<Behavior> behaviors = (AttachableCollection<Behavior>)d.GetValue(BehaviorsProperty);
            if(behaviors == null) {
                behaviors = new AttachableCollection<Behavior>();
                d.SetValue(BehaviorsProperty, behaviors);
            }
            return behaviors;
        }
        public static DataTemplate GetBehaviorsTemplate(DependencyObject d) {
            return (DataTemplate)d.GetValue(BehaviorsProperty);
        }
        public static void SetBehaviorsTemplate(DependencyObject d, DataTemplate template) {
            d.SetValue(BehaviorsTemplateProperty, template);
        }
        static void OnCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            IAttachableObject oldValue = (IAttachableObject)e.OldValue;
            IAttachableObject newValue = (IAttachableObject)e.NewValue;
            if(object.ReferenceEquals(oldValue, newValue)) return;
            if(oldValue != null && oldValue.AssociatedObject != null)
                oldValue.Detach();
            if(newValue != null && d != null) {
                if(newValue.AssociatedObject != null)
                    throw new InvalidOperationException();
                newValue.Attach(d);
            }
        }
        static void OnBehaviorsTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            AttachableCollection<Behavior> objectBehaviors = GetBehaviors(d);
            IList<Behavior> oldItems = d.GetValue(BehaviorsTemplateItemsProperty) as IList<Behavior>;
            DataTemplate newValue = e.NewValue as DataTemplate;
            if(oldItems != null) {
                foreach(Behavior behavior in oldItems)
                    if(objectBehaviors.Contains(behavior))
                        objectBehaviors.Remove(behavior);
            }
            if(newValue == null) {
                d.SetValue(BehaviorsTemplateItemsProperty, null);
                return;
            }

            if(!newValue.IsSealed)
                newValue.Seal();

            IList<Behavior> newItems;
            DependencyObject content = newValue.LoadContent();

            if(content is ContentControl) {
                newItems = new List<Behavior>();
                var behavior = ((ContentControl)content).Content as Behavior;
                ((ContentControl)content).Content = null;
                if(behavior != null)
                    newItems.Add(behavior);
            } else if(content is ItemsControl) {
                var ic = content as ItemsControl;
                newItems = ic.Items.OfType<Behavior>().ToList();
                ic.Items.Clear();
                ic.ItemsSource = null;
            } else
                throw new InvalidOperationException("Use ContentControl or ItemsControl in the template to specify Behaviors.");

            d.SetValue(BehaviorsTemplateItemsProperty, newItems.Count > 0 ? newItems : null);
            foreach(Behavior behavior in newItems)
                objectBehaviors.Add(behavior);
        }
    }
    [AttributeUsage(AttributeTargets.Field)]
    public class IgnoreDependencyPropertiesConsistencyCheckerAttribute : Attribute {
    }
}