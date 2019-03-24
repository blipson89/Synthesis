using System;
using Sitecore.Data.Fields;
using Sitecore.SecurityModel;
using Synthesis.FieldTypes;
using Synthesis.Tests.Utility;
using Xunit;

namespace Synthesis.Tests.FieldTypes
{
	[Trait("Category", "FieldType Tests")]
	public class BooleanFieldTests : IDisposable
    {
        private readonly FieldTestTemplateCreator _creator;
		public BooleanFieldTests()
		{
			_creator = new FieldTestTemplateCreator();
		}
		public void Dispose()
		{
            _creator.Dispose();
		}

		[Fact]
		public void BooleanField_GetValue_WhenTrue()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.BOOLEAN, "1");

				var field = new BooleanField(new LazyField(() => item[TestFields.BOOLEAN], "TEST", TestFields.BOOLEAN), null);
				
				Assert.True(field.Value);
			}
		}

		[Fact]
		public void BooleanField_GetValue_ReturnsFalseWhenBlank()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.BOOLEAN, "");

				var field = new BooleanField(new LazyField(() => item[TestFields.BOOLEAN], "TEST", TestFields.BOOLEAN), null);

				Assert.False(field.Value);
			}
		}

		[Fact]
		public void BooleanField_GetValue_ReturnsFalseWhenZero()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.BOOLEAN, "0");

				var field = new BooleanField(new LazyField(() => item[TestFields.BOOLEAN], "TEST", TestFields.BOOLEAN), null);

				Assert.False(field.Value);
			}
		}

		[Fact]
		public void BooleanField_SetValue_SavesWhenSetTrue()
		{
			using (var item = new TestItemContext())
			{
				var field = new BooleanField(new LazyField(() => item[TestFields.BOOLEAN], "TEST", TestFields.BOOLEAN), null);

				using (new SecurityDisabler())
				{
					field.Value = true;
				}

				CheckboxField sitecoreField = item[TestFields.BOOLEAN];

				Assert.True(sitecoreField.Checked);
			}
		}

		[Fact]
		public void BooleanField_SetValue_SavesWhenSetFalse()
		{
			using (var item = new TestItemContext())
			{
				var field = new BooleanField(new LazyField(() => item[TestFields.BOOLEAN], "TEST", TestFields.BOOLEAN), null);

				using (new SecurityDisabler())
				{
					field.Value = false;
				}

				CheckboxField sitecoreField = item[TestFields.BOOLEAN];

				Assert.False(sitecoreField.Checked);
			}
		}
	}
}
