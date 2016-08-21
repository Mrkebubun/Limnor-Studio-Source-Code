/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms.Design.Behavior;
using System.ComponentModel;
using System.Collections;

namespace Limnor.Drawing2D
{
	public class DrawingItemDesigner : ComponentDesigner
	{
		private Adorner drawItemAdorner = null;
		// References to designer services, for convenience.
		private IComponentChangeService changeService = null;
		private ISelectionService selectionService = null;
		private BehaviorService behaviorSvc = null;
		//
		public DrawingItemDesigner()
		{
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.behaviorSvc != null)
				{
					// Remove the adorners added by this designer from
					// the BehaviorService.Adorners collection.
					this.behaviorSvc.Adorners.Remove(this.drawItemAdorner);
				}
			}

			base.Dispose(disposing);
		}
		// This method is where the designer initializes its state when
		// it is created.
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);

			// Connect to various designer services.
			InitializeServices();

			// Initialize adorners.
			this.InitializeDrawItemAdorner();

		}
		// This demonstrates changing the appearance of a control while
		// it is being designed. In this case, the BackColor property is
		// set to LightBlue. 

		public override void InitializeNewComponent(IDictionary defaultValues)
		{
			base.InitializeNewComponent(defaultValues);

			//PropertyDescriptor colorPropDesc =
			//    TypeDescriptor.GetProperties(Component)["BackColor"];

			//if (colorPropDesc != null &&
			//    colorPropDesc.PropertyType == typeof(Color) &&
			//    !colorPropDesc.IsReadOnly &&
			//    colorPropDesc.IsBrowsable)
			//{
			//    colorPropDesc.SetValue(Component, Color.LightBlue);
			//}
		}
		// This utility method creates an adorner for the anchor glyphs.
		// It then creates four AnchorGlyph objects and adds them to 
		// the adorner's Glyphs collection.
		private void InitializeDrawItemAdorner()
		{
			this.drawItemAdorner = new Adorner();
			this.behaviorSvc.Adorners.Add(this.drawItemAdorner);

			this.drawItemAdorner.Glyphs.Add(new DrawingItemDesignerGlyph(
				this.behaviorSvc,
				this.changeService,
				this.selectionService,
				this,
				this.drawItemAdorner)
				);

		}
		// This utility method connects the designer to various services.
		// These references are cached for convenience.
		private void InitializeServices()
		{
			// Acquire a reference to IComponentChangeService.
			this.changeService =
				GetService(typeof(IComponentChangeService))
				as IComponentChangeService;

			// Acquire a reference to ISelectionService.
			this.selectionService =
				GetService(typeof(ISelectionService))
				as ISelectionService;

			// Acquire a reference to BehaviorService.
			this.behaviorSvc =
				GetService(typeof(BehaviorService))
				as BehaviorService;
		}
	}
}
