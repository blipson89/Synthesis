using System;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.SecurityModel;
using Synthesis.FieldTypes;
using Synthesis.Tests.Utility;
using Xunit;

namespace Synthesis.Tests.FieldTypes
{
    [Trait("Category", "FieldType Tests")]
    public class DateTimeFieldTests : IDisposable
	{
        private readonly FieldTestTemplateCreator _creator;
        public DateTimeFieldTests()
		{
			_creator = new FieldTestTemplateCreator();
		}

		public void Dispose()
		{
			_creator.Dispose();
		}

		[Fact]
		public void DateTimeField_GetValue_ReturnsDateMinWhenEmpty()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.DATETIME, string.Empty);

				var field = new DateTimeField(new LazyField(() => item[TestFields.DATETIME], "TEST", TestFields.DATETIME), null);

				Assert.Equal(DateTime.MinValue, field.Value);
			}
		}

		[Fact]
		public void DateTimeField_GetValue_ReturnsValidValue()
		{
			using (var item = new TestItemContext())
			{
				var dateSet = DateUtil.IsoDateToDateTime(DateUtil.ToIsoDate(DateTime.UtcNow)); // convert now to sitecore format and back to make sure the rounding is correct - sitecore doesn't have quite the same precision (no msec) as DateTime.Now
				item.SetField(TestFields.DATETIME, DateUtil.ToIsoDate(dateSet));

				var field = new DateTimeField(new LazyField(() => item[TestFields.DATETIME], "TEST", TestFields.DATETIME), null);

				Assert.Equal(dateSet, field.Value);
			}
		}

		[Fact]
		public void DateTimeField_SetValue_SavesDate()
		{
			using (var item = new TestItemContext())
			{
				var field = new DateTimeField(new LazyField(() => item[TestFields.DATETIME], "TEST", TestFields.DATETIME), null);
				var dateSet = DateUtil.IsoDateToDateTime(DateUtil.ToIsoDate(DateTime.UtcNow)); // convert now to sitecore format and back to make sure the rounding is correct - sitecore doesn't have quite the same precision (no msec) as DateTime.Now
				
				using (new SecurityDisabler())
				{
					field.Value = dateSet;
				}

				DateField sitecoreField = item[TestFields.DATETIME];

				Assert.Equal(dateSet, sitecoreField.DateTime);
			}
		}

		[Fact]
		public void DateTimeField_SetValue_SavesMaxDate()
		{
			using (var item = new TestItemContext())
			{
				var field = new DateTimeField(new LazyField(() => item[TestFields.DATETIME], "TEST", TestFields.DATETIME), null);
				var dateSet = DateUtil.IsoDateToDateTime(DateUtil.ToIsoDate(DateTime.MaxValue.ToUniversalTime())); // convert maxvalue to sitecore format and back to make sure the rounding is correct - sitecore doesn't have quite the same precision (no msec) as DateTime.Now
				
				using (new SecurityDisabler())
				{
					field.Value = dateSet;
				}

				DateField sitecoreField = item[TestFields.DATETIME];

				Assert.Equal(dateSet, sitecoreField.DateTime);
			}
		}

		[Fact]
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

				Assert.Equal(DateTime.MinValue, sitecoreField.DateTime);
			}
		}

		[Fact]
		public void DateTimeField_HasValue_WhenTrue()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.DATETIME, DateUtil.IsoNow);

				var field = new DateTimeField(new LazyField(() => item[TestFields.DATETIME], "TEST", TestFields.DATETIME), null);

				Assert.True(field.HasValue);
			}
		}

		[Fact]
		public void DateTimeField_HasValue_WhenFalse()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.DATETIME, string.Empty);

				var field = new DateTimeField(new LazyField(() => item[TestFields.DATETIME], "TEST", TestFields.DATETIME), null);

				Assert.False(field.HasValue);
			}
		}
	}
}
