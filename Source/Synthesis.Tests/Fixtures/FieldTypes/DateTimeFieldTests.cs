using System;
using NUnit.Framework;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.SecurityModel;
using Synthesis.FieldTypes;
using Synthesis.Tests.Utility;

namespace Synthesis.Tests.Fixtures.FieldTypes
{
	[TestFixture]
	[Category("FieldType Tests")]
	public class DateTimeFieldTests
	{
		[TestFixtureSetUp]
		public void SetUpTestTemplate()
		{
			new FieldTestTemplateCreator().CreateSampleTemplate();
		}

		[TestFixtureTearDown]
		public void TearDownTestTemplate()
		{
			new FieldTestTemplateCreator().DeleteSampleTemplate();
		}

		[Test]
		public void DateTimeField_GetValue_ReturnsDateMinWhenEmpty()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.DATETIME, string.Empty);

				var field = new DateTimeField(new LazyField(() => item[TestFields.DATETIME], "TEST", TestFields.DATETIME), null);

				Assert.AreEqual(field.Value, DateTime.MinValue);
			}
		}

		[Test]
		public void DateTimeField_GetValue_ReturnsValidValue()
		{
			using (var item = new TestItemContext())
			{
				var dateSet = DateUtil.IsoDateToDateTime(DateUtil.ToIsoDate(DateTime.Now)); // convert now to sitecore format and back to make sure the rounding is correct - sitecore doesn't have quite the same precision (no msec) as DateTime.Now
				item.SetField(TestFields.DATETIME, DateUtil.ToIsoDate(dateSet));

				var field = new DateTimeField(new LazyField(() => item[TestFields.DATETIME], "TEST", TestFields.DATETIME), null);

				Assert.AreEqual(field.Value, dateSet);
			}
		}

		[Test]
		public void DateTimeField_SetValue_SavesDate()
		{
			using (var item = new TestItemContext())
			{
				var field = new DateTimeField(new LazyField(() => item[TestFields.DATETIME], "TEST", TestFields.DATETIME), null);
				var dateSet = DateUtil.IsoDateToDateTime(DateUtil.ToIsoDate(DateTime.Now)); // convert now to sitecore format and back to make sure the rounding is correct - sitecore doesn't have quite the same precision (no msec) as DateTime.Now
				
				using (new SecurityDisabler())
				{
					field.Value = dateSet;
				}

				DateField sitecoreField = item[TestFields.DATETIME];

				Assert.AreEqual(dateSet, sitecoreField.DateTime);
			}
		}

		[Test]
		public void DateTimeField_SetValue_SavesMaxDate()
		{
			using (var item = new TestItemContext())
			{
				var field = new DateTimeField(new LazyField(() => item[TestFields.DATETIME], "TEST", TestFields.DATETIME), null);
				var dateSet = DateUtil.IsoDateToDateTime(DateUtil.ToIsoDate(DateTime.MaxValue)); // convert maxvalue to sitecore format and back to make sure the rounding is correct - sitecore doesn't have quite the same precision (no msec) as DateTime.Now
				
				using (new SecurityDisabler())
				{
					field.Value = dateSet;
				}

				DateField sitecoreField = item[TestFields.DATETIME];

				Assert.AreEqual(dateSet, sitecoreField.DateTime);
			}
		}

		[Test]
		public void DateTimeField_SetValue_SavesMinDate()
		{
			using (var item = new TestItemContext())
			{
				var field = new DateTimeField(new LazyField(() => item[TestFields.DATETIME], "TEST", TestFields.DATETIME), null);

				using (new SecurityDisabler())
				{
					field.Value = DateTime.MinValue;
				}

				DateField sitecoreField = item[TestFields.DATETIME];

				Assert.AreEqual(DateTime.MinValue, sitecoreField.DateTime);
			}
		}

		[Test]
		public void DateTimeField_HasValue_WhenTrue()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.DATETIME, DateUtil.IsoNow);

				var field = new DateTimeField(new LazyField(() => item[TestFields.DATETIME], "TEST", TestFields.DATETIME), null);

				Assert.IsTrue(field.HasValue);
			}
		}

		[Test]
		public void DateTimeField_HasValue_WhenFalse()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.DATETIME, string.Empty);

				var field = new DateTimeField(new LazyField(() => item[TestFields.DATETIME], "TEST", TestFields.DATETIME), null);

				Assert.IsFalse(field.HasValue);
			}
		}
	}
}
