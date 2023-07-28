using System.Text;
using IdentifierCasing;
using IdentifierCasing.Utility;

namespace IdentifierCasing.Test
{
    [TestClass]
    public class TestIdentifierCasingTranslator
    {
        [TestMethod]
        public void CasingTranslator()
        {
            Dictionary<CasingStyle, string> testStrings = new();
            testStrings[CasingStyle.Lower] = "this is a test";
            testStrings[CasingStyle.Upper] = "THIS IS A TEST";
            testStrings[CasingStyle.Pascal] = "ThisIsATest";
            testStrings[CasingStyle.Camel] = "thisIsATest";
            testStrings[CasingStyle.Kebab] = "this-is-a-test";
            testStrings[CasingStyle.Snake] = "this_is_a_test";
            testStrings[CasingStyle.Cobol] = "THIS-IS-A-TEST";
            testStrings[CasingStyle.Macro] = "THIS_IS_A_TEST";
            testStrings[CasingStyle.Train] = "This-Is-A-Test";
            testStrings[CasingStyle.Spreadsheet] = "This Is A Test";
            testStrings[CasingStyle.Mixed] = "this-Is_aTest";
            // detect eacnh casing format
            foreach (KeyValuePair<CasingStyle, string> kvp in testStrings)
            {
                Assert.AreEqual(kvp.Key, IdentifierCasing.CasingTranslator.DetectIdentifierCasing(kvp.Value));
            }
            // translate each string into each casing (except Mixed)
            foreach (KeyValuePair<CasingStyle, string> kvp in testStrings)
            {
                foreach (CasingStyle newCasing in EnumExtensions.EnumUniqueValues<CasingStyle>())
                {
                    if (newCasing == CasingStyle.Mixed) continue;   // skip mixed casing as an output
                    Assert.AreEqual(testStrings[newCasing], IdentifierCasing.CasingTranslator.Translate(kvp.Value, newCasing));
                }
            }
            // write the PascalCasing identifier to a string with a specified casing and make sure it is correct
            foreach (CasingStyle newCasing in EnumExtensions.EnumUniqueValues<CasingStyle>())
            {
                if (newCasing == CasingStyle.Mixed) continue;   // skip mixed casing as an output
                StringBuilder written = new();
                using (StringWriter writer = new(written))
                {
                    IdentifierCasing.CasingTranslator.WriteCasedIdentifier(writer, newCasing, testStrings[CasingStyle.Pascal]);
                    writer.Flush();
                }
                Assert.AreEqual(testStrings[newCasing], written.ToString());
            }
        }
        [TestMethod]
        public void CasingTranslatorDashOrUnderscoreLeadingEndingIdentifier()
        {
            Assert.AreEqual("this-is-a-test ", IdentifierCasing.CasingTranslator.Translate("ThisIsATest ", CasingStyle.Kebab));
            Assert.AreEqual("this-is-a-test-", IdentifierCasing.CasingTranslator.Translate("ThisIsATest-", CasingStyle.Kebab));
            Assert.AreEqual("this-is-a-test_", IdentifierCasing.CasingTranslator.Translate("ThisIsATest_", CasingStyle.Kebab));
            Assert.AreEqual(" this-is-a-test", IdentifierCasing.CasingTranslator.Translate(" ThisIsATest", CasingStyle.Kebab));
            Assert.AreEqual("-this-is-a-test", IdentifierCasing.CasingTranslator.Translate("-ThisIsATest", CasingStyle.Kebab));
            Assert.AreEqual("_this-is-a-test", IdentifierCasing.CasingTranslator.Translate("_ThisIsATest", CasingStyle.Kebab));
            Assert.AreEqual("this_is_a_test ", IdentifierCasing.CasingTranslator.Translate("ThisIsATest ", CasingStyle.Snake));
            Assert.AreEqual("this_is_a_test-", IdentifierCasing.CasingTranslator.Translate("ThisIsATest-", CasingStyle.Snake));
            Assert.AreEqual("this_is_a_test_", IdentifierCasing.CasingTranslator.Translate("ThisIsATest_", CasingStyle.Snake));
            Assert.AreEqual(" this_is_a_test", IdentifierCasing.CasingTranslator.Translate(" ThisIsATest", CasingStyle.Snake));
            Assert.AreEqual("-this_is_a_test", IdentifierCasing.CasingTranslator.Translate("-ThisIsATest", CasingStyle.Snake));
            Assert.AreEqual("_this_is_a_test", IdentifierCasing.CasingTranslator.Translate("_ThisIsATest", CasingStyle.Snake));
            Assert.AreEqual("THIS-IS-A-TEST ", IdentifierCasing.CasingTranslator.Translate("ThisIsATest ", CasingStyle.Cobol));
            Assert.AreEqual("THIS-IS-A-TEST-", IdentifierCasing.CasingTranslator.Translate("ThisIsATest-", CasingStyle.Cobol));
            Assert.AreEqual("THIS-IS-A-TEST_", IdentifierCasing.CasingTranslator.Translate("ThisIsATest_", CasingStyle.Cobol));
            Assert.AreEqual(" THIS-IS-A-TEST", IdentifierCasing.CasingTranslator.Translate(" ThisIsATest", CasingStyle.Cobol));
            Assert.AreEqual("-THIS-IS-A-TEST", IdentifierCasing.CasingTranslator.Translate("-ThisIsATest", CasingStyle.Cobol));
            Assert.AreEqual("_THIS-IS-A-TEST", IdentifierCasing.CasingTranslator.Translate("_ThisIsATest", CasingStyle.Cobol));
            Assert.AreEqual("THIS_IS_A_TEST ", IdentifierCasing.CasingTranslator.Translate("ThisIsATest ", CasingStyle.Macro));
            Assert.AreEqual("THIS_IS_A_TEST-", IdentifierCasing.CasingTranslator.Translate("ThisIsATest-", CasingStyle.Macro));
            Assert.AreEqual("THIS_IS_A_TEST_", IdentifierCasing.CasingTranslator.Translate("ThisIsATest_", CasingStyle.Macro));
            Assert.AreEqual(" THIS_IS_A_TEST", IdentifierCasing.CasingTranslator.Translate(" ThisIsATest", CasingStyle.Macro));
            Assert.AreEqual("-THIS_IS_A_TEST", IdentifierCasing.CasingTranslator.Translate("-ThisIsATest", CasingStyle.Macro));
            Assert.AreEqual("_THIS_IS_A_TEST", IdentifierCasing.CasingTranslator.Translate("_ThisIsATest", CasingStyle.Macro));
            Assert.AreEqual("thisIsATest ", IdentifierCasing.CasingTranslator.Translate("ThisIsATest ", CasingStyle.Camel));
            Assert.AreEqual("thisIsATest-", IdentifierCasing.CasingTranslator.Translate("ThisIsATest-", CasingStyle.Camel));
            Assert.AreEqual("thisIsATest_", IdentifierCasing.CasingTranslator.Translate("ThisIsATest_", CasingStyle.Camel));
            Assert.AreEqual(" thisIsATest", IdentifierCasing.CasingTranslator.Translate(" ThisIsATest", CasingStyle.Camel));
            Assert.AreEqual("-thisIsATest", IdentifierCasing.CasingTranslator.Translate("-ThisIsATest", CasingStyle.Camel));
            Assert.AreEqual("_thisIsATest", IdentifierCasing.CasingTranslator.Translate("_ThisIsATest", CasingStyle.Camel));
            Assert.AreEqual("ThisIsATest ", IdentifierCasing.CasingTranslator.Translate("ThisIsATest ", CasingStyle.Pascal));
            Assert.AreEqual("ThisIsATest-", IdentifierCasing.CasingTranslator.Translate("ThisIsATest-", CasingStyle.Pascal));
            Assert.AreEqual("ThisIsATest_", IdentifierCasing.CasingTranslator.Translate("ThisIsATest_", CasingStyle.Pascal));
            Assert.AreEqual(" ThisIsATest", IdentifierCasing.CasingTranslator.Translate(" ThisIsATest", CasingStyle.Pascal));
            Assert.AreEqual("-ThisIsATest", IdentifierCasing.CasingTranslator.Translate("-ThisIsATest", CasingStyle.Pascal));
            Assert.AreEqual("_ThisIsATest", IdentifierCasing.CasingTranslator.Translate("_ThisIsATest", CasingStyle.Pascal));
            Assert.AreEqual("This-Is-A-Test ", IdentifierCasing.CasingTranslator.Translate("ThisIsATest ", CasingStyle.Train));
            Assert.AreEqual("This-Is-A-Test-", IdentifierCasing.CasingTranslator.Translate("ThisIsATest-", CasingStyle.Train));
            Assert.AreEqual("This-Is-A-Test_", IdentifierCasing.CasingTranslator.Translate("ThisIsATest_", CasingStyle.Train));
            Assert.AreEqual("This-Is-A-Test  ", IdentifierCasing.CasingTranslator.Translate("ThisIsATest  ", CasingStyle.Train));
            Assert.AreEqual("This-Is-A-Test--", IdentifierCasing.CasingTranslator.Translate("ThisIsATest--", CasingStyle.Train));
            Assert.AreEqual("This-Is-A-Test__", IdentifierCasing.CasingTranslator.Translate("ThisIsATest__", CasingStyle.Train));
            Assert.AreEqual(" This-Is-A-Test", IdentifierCasing.CasingTranslator.Translate(" ThisIsATest", CasingStyle.Train));
            Assert.AreEqual("-This-Is-A-Test", IdentifierCasing.CasingTranslator.Translate("-ThisIsATest", CasingStyle.Train));
            Assert.AreEqual("_This-Is-A-Test", IdentifierCasing.CasingTranslator.Translate("_ThisIsATest", CasingStyle.Train));
            Assert.AreEqual("  This-Is-A-Test", IdentifierCasing.CasingTranslator.Translate("  ThisIsATest", CasingStyle.Train));
            Assert.AreEqual("--This-Is-A-Test", IdentifierCasing.CasingTranslator.Translate("--ThisIsATest", CasingStyle.Train));
            Assert.AreEqual("__This-Is-A-Test", IdentifierCasing.CasingTranslator.Translate("__ThisIsATest", CasingStyle.Train));
            Assert.AreEqual("   This-Is-A-Test   ", IdentifierCasing.CasingTranslator.Translate("   ThisIsATest   ", CasingStyle.Train));
            Assert.AreEqual("---This-Is-A-Test---", IdentifierCasing.CasingTranslator.Translate("---ThisIsATest---", CasingStyle.Train));
            Assert.AreEqual("___This-Is-A-Test___", IdentifierCasing.CasingTranslator.Translate("___ThisIsATest___", CasingStyle.Train));
            Assert.AreEqual("This Is A Test ", IdentifierCasing.CasingTranslator.Translate("ThisIsATest ", CasingStyle.Spreadsheet));
            Assert.AreEqual("This Is A Test-", IdentifierCasing.CasingTranslator.Translate("ThisIsATest-", CasingStyle.Spreadsheet));
            Assert.AreEqual("This Is A Test_", IdentifierCasing.CasingTranslator.Translate("ThisIsATest_", CasingStyle.Spreadsheet));
            Assert.AreEqual("This Is A Test  ", IdentifierCasing.CasingTranslator.Translate("ThisIsATest  ", CasingStyle.Spreadsheet));
            Assert.AreEqual("This Is A Test--", IdentifierCasing.CasingTranslator.Translate("ThisIsATest--", CasingStyle.Spreadsheet));
            Assert.AreEqual("This Is A Test__", IdentifierCasing.CasingTranslator.Translate("ThisIsATest__", CasingStyle.Spreadsheet));
            Assert.AreEqual(" This Is A Test", IdentifierCasing.CasingTranslator.Translate(" ThisIsATest", CasingStyle.Spreadsheet));
            Assert.AreEqual("-This Is A Test", IdentifierCasing.CasingTranslator.Translate("-ThisIsATest", CasingStyle.Spreadsheet));
            Assert.AreEqual("_This Is A Test", IdentifierCasing.CasingTranslator.Translate("_ThisIsATest", CasingStyle.Spreadsheet));
            Assert.AreEqual("  This Is A Test", IdentifierCasing.CasingTranslator.Translate("  ThisIsATest", CasingStyle.Spreadsheet));
            Assert.AreEqual("--This Is A Test", IdentifierCasing.CasingTranslator.Translate("--ThisIsATest", CasingStyle.Spreadsheet));
            Assert.AreEqual("__This Is A Test", IdentifierCasing.CasingTranslator.Translate("__ThisIsATest", CasingStyle.Spreadsheet));
            Assert.AreEqual("   This Is A Test   ", IdentifierCasing.CasingTranslator.Translate("   ThisIsATest   ", CasingStyle.Spreadsheet));
            Assert.AreEqual("---This Is A Test---", IdentifierCasing.CasingTranslator.Translate("---ThisIsATest---", CasingStyle.Spreadsheet));
            Assert.AreEqual("___This Is A Test___", IdentifierCasing.CasingTranslator.Translate("___ThisIsATest___", CasingStyle.Spreadsheet));
        }
        [TestMethod]
        public void CasingTranslatorPunctuationLeadingEndingIdentifier()
        {
            Assert.AreEqual("this-is-a-test—", IdentifierCasing.CasingTranslator.Translate("ThisIsATest—", CasingStyle.Kebab));
            Assert.AreEqual("this-is-a-test@", IdentifierCasing.CasingTranslator.Translate("ThisIsATest@", CasingStyle.Kebab));
            Assert.AreEqual("this-is-a-test¿", IdentifierCasing.CasingTranslator.Translate("ThisIsATest¿", CasingStyle.Kebab));
            Assert.AreEqual("—this-is-a-test", IdentifierCasing.CasingTranslator.Translate("—ThisIsATest", CasingStyle.Kebab));
            Assert.AreEqual("@this-is-a-test", IdentifierCasing.CasingTranslator.Translate("@ThisIsATest", CasingStyle.Kebab));
            Assert.AreEqual("¿this-is-a-test", IdentifierCasing.CasingTranslator.Translate("¿ThisIsATest", CasingStyle.Kebab));
            Assert.AreEqual("this_is_a_test—", IdentifierCasing.CasingTranslator.Translate("ThisIsATest—", CasingStyle.Snake));
            Assert.AreEqual("this_is_a_test@", IdentifierCasing.CasingTranslator.Translate("ThisIsATest@", CasingStyle.Snake));
            Assert.AreEqual("this_is_a_test¿", IdentifierCasing.CasingTranslator.Translate("ThisIsATest¿", CasingStyle.Snake));
            Assert.AreEqual("—this_is_a_test", IdentifierCasing.CasingTranslator.Translate("—ThisIsATest", CasingStyle.Snake));
            Assert.AreEqual("@this_is_a_test", IdentifierCasing.CasingTranslator.Translate("@ThisIsATest", CasingStyle.Snake));
            Assert.AreEqual("¿this_is_a_test", IdentifierCasing.CasingTranslator.Translate("¿ThisIsATest", CasingStyle.Snake));
            Assert.AreEqual("THIS-IS-A-TEST—", IdentifierCasing.CasingTranslator.Translate("ThisIsATest—", CasingStyle.Cobol));
            Assert.AreEqual("THIS-IS-A-TEST@", IdentifierCasing.CasingTranslator.Translate("ThisIsATest@", CasingStyle.Cobol));
            Assert.AreEqual("THIS-IS-A-TEST¿", IdentifierCasing.CasingTranslator.Translate("ThisIsATest¿", CasingStyle.Cobol));
            Assert.AreEqual("—THIS-IS-A-TEST", IdentifierCasing.CasingTranslator.Translate("—ThisIsATest", CasingStyle.Cobol));
            Assert.AreEqual("@THIS-IS-A-TEST", IdentifierCasing.CasingTranslator.Translate("@ThisIsATest", CasingStyle.Cobol));
            Assert.AreEqual("¿THIS-IS-A-TEST", IdentifierCasing.CasingTranslator.Translate("¿ThisIsATest", CasingStyle.Cobol));
            Assert.AreEqual("THIS_IS_A_TEST—", IdentifierCasing.CasingTranslator.Translate("ThisIsATest—", CasingStyle.Macro));
            Assert.AreEqual("THIS_IS_A_TEST@", IdentifierCasing.CasingTranslator.Translate("ThisIsATest@", CasingStyle.Macro));
            Assert.AreEqual("THIS_IS_A_TEST¿", IdentifierCasing.CasingTranslator.Translate("ThisIsATest¿", CasingStyle.Macro));
            Assert.AreEqual("—THIS_IS_A_TEST", IdentifierCasing.CasingTranslator.Translate("—ThisIsATest", CasingStyle.Macro));
            Assert.AreEqual("@THIS_IS_A_TEST", IdentifierCasing.CasingTranslator.Translate("@ThisIsATest", CasingStyle.Macro));
            Assert.AreEqual("¿THIS_IS_A_TEST", IdentifierCasing.CasingTranslator.Translate("¿ThisIsATest", CasingStyle.Macro));
            Assert.AreEqual("thisIsATest—", IdentifierCasing.CasingTranslator.Translate("ThisIsATest—", CasingStyle.Camel));
            Assert.AreEqual("thisIsATest@", IdentifierCasing.CasingTranslator.Translate("ThisIsATest@", CasingStyle.Camel));
            Assert.AreEqual("thisIsATest¿", IdentifierCasing.CasingTranslator.Translate("ThisIsATest¿", CasingStyle.Camel));
            Assert.AreEqual("—thisIsATest", IdentifierCasing.CasingTranslator.Translate("—ThisIsATest", CasingStyle.Camel));
            Assert.AreEqual("@thisIsATest", IdentifierCasing.CasingTranslator.Translate("@ThisIsATest", CasingStyle.Camel));
            Assert.AreEqual("¿thisIsATest", IdentifierCasing.CasingTranslator.Translate("¿ThisIsATest", CasingStyle.Camel));
            Assert.AreEqual("ThisIsATest—", IdentifierCasing.CasingTranslator.Translate("ThisIsATest—", CasingStyle.Pascal));
            Assert.AreEqual("ThisIsATest@", IdentifierCasing.CasingTranslator.Translate("ThisIsATest@", CasingStyle.Pascal));
            Assert.AreEqual("ThisIsATest¿", IdentifierCasing.CasingTranslator.Translate("ThisIsATest¿", CasingStyle.Pascal));
            Assert.AreEqual("—ThisIsATest", IdentifierCasing.CasingTranslator.Translate("—ThisIsATest", CasingStyle.Pascal));
            Assert.AreEqual("@ThisIsATest", IdentifierCasing.CasingTranslator.Translate("@ThisIsATest", CasingStyle.Pascal));
            Assert.AreEqual("¿ThisIsATest", IdentifierCasing.CasingTranslator.Translate("¿ThisIsATest", CasingStyle.Pascal));
            Assert.AreEqual("This-Is-A-Test—", IdentifierCasing.CasingTranslator.Translate("ThisIsATest—", CasingStyle.Train));
            Assert.AreEqual("This-Is-A-Test@", IdentifierCasing.CasingTranslator.Translate("ThisIsATest@", CasingStyle.Train));
            Assert.AreEqual("This-Is-A-Test¿", IdentifierCasing.CasingTranslator.Translate("ThisIsATest¿", CasingStyle.Train));
            Assert.AreEqual("This-Is-A-Test「—", IdentifierCasing.CasingTranslator.Translate("ThisIsATest「—", CasingStyle.Train));
            Assert.AreEqual("This-Is-A-Test@@", IdentifierCasing.CasingTranslator.Translate("ThisIsATest@@", CasingStyle.Train));
            Assert.AreEqual("This-Is-A-Test¿¿", IdentifierCasing.CasingTranslator.Translate("ThisIsATest¿¿", CasingStyle.Train));
            Assert.AreEqual("—This-Is-A-Test", IdentifierCasing.CasingTranslator.Translate("—ThisIsATest", CasingStyle.Train));
            Assert.AreEqual("@This-Is-A-Test", IdentifierCasing.CasingTranslator.Translate("@ThisIsATest", CasingStyle.Train));
            Assert.AreEqual("¿This-Is-A-Test", IdentifierCasing.CasingTranslator.Translate("¿ThisIsATest", CasingStyle.Train));
            Assert.AreEqual("「—This-Is-A-Test", IdentifierCasing.CasingTranslator.Translate("「—ThisIsATest", CasingStyle.Train));
            Assert.AreEqual("@@This-Is-A-Test", IdentifierCasing.CasingTranslator.Translate("@@ThisIsATest", CasingStyle.Train));
            Assert.AreEqual("¿¿This-Is-A-Test", IdentifierCasing.CasingTranslator.Translate("¿¿ThisIsATest", CasingStyle.Train));
            Assert.AreEqual("๚「—This-Is-A-Test๚「—", IdentifierCasing.CasingTranslator.Translate("๚「—ThisIsATest๚「—", CasingStyle.Train));
            Assert.AreEqual("@@@This-Is-A-Test@@@", IdentifierCasing.CasingTranslator.Translate("@@@ThisIsATest@@@", CasingStyle.Train));
            Assert.AreEqual("¿¿¿This-Is-A-Test¿¿¿", IdentifierCasing.CasingTranslator.Translate("¿¿¿ThisIsATest¿¿¿", CasingStyle.Train));
            Assert.AreEqual("This Is A Test—", IdentifierCasing.CasingTranslator.Translate("ThisIsATest—", CasingStyle.Spreadsheet));
            Assert.AreEqual("This Is A Test@", IdentifierCasing.CasingTranslator.Translate("ThisIsATest@", CasingStyle.Spreadsheet));
            Assert.AreEqual("This Is A Test¿", IdentifierCasing.CasingTranslator.Translate("ThisIsATest¿", CasingStyle.Spreadsheet));
            Assert.AreEqual("This Is A Test「—", IdentifierCasing.CasingTranslator.Translate("ThisIsATest「—", CasingStyle.Spreadsheet));
            Assert.AreEqual("This Is A Test@@", IdentifierCasing.CasingTranslator.Translate("ThisIsATest@@", CasingStyle.Spreadsheet));
            Assert.AreEqual("This Is A Test¿¿", IdentifierCasing.CasingTranslator.Translate("ThisIsATest¿¿", CasingStyle.Spreadsheet));
            Assert.AreEqual("—This Is A Test", IdentifierCasing.CasingTranslator.Translate("—ThisIsATest", CasingStyle.Spreadsheet));
            Assert.AreEqual("@This Is A Test", IdentifierCasing.CasingTranslator.Translate("@ThisIsATest", CasingStyle.Spreadsheet));
            Assert.AreEqual("¿This Is A Test", IdentifierCasing.CasingTranslator.Translate("¿ThisIsATest", CasingStyle.Spreadsheet));
            Assert.AreEqual("「—This Is A Test", IdentifierCasing.CasingTranslator.Translate("「—ThisIsATest", CasingStyle.Spreadsheet));
            Assert.AreEqual("@@This Is A Test", IdentifierCasing.CasingTranslator.Translate("@@ThisIsATest", CasingStyle.Spreadsheet));
            Assert.AreEqual("¿¿This Is A Test", IdentifierCasing.CasingTranslator.Translate("¿¿ThisIsATest", CasingStyle.Spreadsheet));
            Assert.AreEqual("๚「—This Is A Test๚「—", IdentifierCasing.CasingTranslator.Translate("๚「—ThisIsATest๚「—", CasingStyle.Spreadsheet));
            Assert.AreEqual("@@@This Is A Test@@@", IdentifierCasing.CasingTranslator.Translate("@@@ThisIsATest@@@", CasingStyle.Spreadsheet));
            Assert.AreEqual("¿¿¿This Is A Test¿¿¿", IdentifierCasing.CasingTranslator.Translate("¿¿¿ThisIsATest¿¿¿", CasingStyle.Spreadsheet));
        }
        [TestMethod]
        public void CasingTranslatorTilde()
        {
            Assert.AreEqual("this-is-a-test~", IdentifierCasing.CasingTranslator.Translate("ThisIsATest~", CasingStyle.Kebab));
            Assert.AreEqual("~this-is-a-test", IdentifierCasing.CasingTranslator.Translate("~ThisIsATest", CasingStyle.Kebab));
            Assert.AreEqual("this_is_a_test~", IdentifierCasing.CasingTranslator.Translate("ThisIsATest~", CasingStyle.Snake));
            Assert.AreEqual("~this_is_a_test", IdentifierCasing.CasingTranslator.Translate("~ThisIsATest", CasingStyle.Snake));
            Assert.AreEqual("THIS-IS-A-TEST~", IdentifierCasing.CasingTranslator.Translate("ThisIsATest~", CasingStyle.Cobol));
            Assert.AreEqual("~THIS-IS-A-TEST", IdentifierCasing.CasingTranslator.Translate("~ThisIsATest", CasingStyle.Cobol));
            Assert.AreEqual("THIS_IS_A_TEST~", IdentifierCasing.CasingTranslator.Translate("ThisIsATest~", CasingStyle.Macro));
            Assert.AreEqual("~THIS_IS_A_TEST", IdentifierCasing.CasingTranslator.Translate("~ThisIsATest", CasingStyle.Macro));
            Assert.AreEqual("thisIsATest~", IdentifierCasing.CasingTranslator.Translate("ThisIsATest~", CasingStyle.Camel));
            Assert.AreEqual("~thisIsATest", IdentifierCasing.CasingTranslator.Translate("~ThisIsATest", CasingStyle.Camel));
            Assert.AreEqual("ThisIsATest~", IdentifierCasing.CasingTranslator.Translate("ThisIsATest~", CasingStyle.Pascal));
            Assert.AreEqual("~ThisIsATest", IdentifierCasing.CasingTranslator.Translate("~ThisIsATest", CasingStyle.Pascal));
            Assert.AreEqual("This-Is-A-Test~", IdentifierCasing.CasingTranslator.Translate("ThisIsATest~", CasingStyle.Train));
            Assert.AreEqual("This-Is-A-Test~~", IdentifierCasing.CasingTranslator.Translate("ThisIsATest~~", CasingStyle.Train));
            Assert.AreEqual("~This-Is-A-Test", IdentifierCasing.CasingTranslator.Translate("~ThisIsATest", CasingStyle.Train));
            Assert.AreEqual("~~This-Is-A-Test", IdentifierCasing.CasingTranslator.Translate("~~ThisIsATest", CasingStyle.Train));
            Assert.AreEqual("~~~This-Is-A-Test~~~", IdentifierCasing.CasingTranslator.Translate("~~~ThisIsATest~~~", CasingStyle.Train));
            Assert.AreEqual("This Is A Test~", IdentifierCasing.CasingTranslator.Translate("ThisIsATest~", CasingStyle.Spreadsheet));
            Assert.AreEqual("This Is A Test~~", IdentifierCasing.CasingTranslator.Translate("ThisIsATest~~", CasingStyle.Spreadsheet));
            Assert.AreEqual("~This Is A Test", IdentifierCasing.CasingTranslator.Translate("~ThisIsATest", CasingStyle.Spreadsheet));
            Assert.AreEqual("~~This Is A Test", IdentifierCasing.CasingTranslator.Translate("~~ThisIsATest", CasingStyle.Spreadsheet));
            Assert.AreEqual("~~~This Is A Test~~~", IdentifierCasing.CasingTranslator.Translate("~~~ThisIsATest~~~", CasingStyle.Spreadsheet));
        }
        [TestMethod]
        public void CasingTranslatorAllDashOrUnderscoreIdentifiers()
        {
            // write the PascalCasing identifier to a string with a specified casing and make sure it is correct
            foreach (CasingStyle newCasing in EnumExtensions.EnumUniqueValues<CasingStyle>())
            {
                if (newCasing == CasingStyle.Mixed) continue;   // skip mixed casing as an output
                Assert.AreEqual("-", IdentifierCasing.CasingTranslator.Translate("-", newCasing));
                Assert.AreEqual("---", IdentifierCasing.CasingTranslator.Translate("---", newCasing));
                Assert.AreEqual("_", IdentifierCasing.CasingTranslator.Translate("_", newCasing));
                Assert.AreEqual("_____", IdentifierCasing.CasingTranslator.Translate("_____", newCasing));
                Assert.AreEqual(" ", IdentifierCasing.CasingTranslator.Translate(" ", newCasing));
                Assert.AreEqual("   ", IdentifierCasing.CasingTranslator.Translate("   ", newCasing));
                Assert.AreEqual("_____    -----", IdentifierCasing.CasingTranslator.Translate("_____    -----", newCasing));
            }
        }
        [TestMethod]
        public void CasingTranslatorReadCasedIdentifier()
        {
            Dictionary<CasingStyle, string> testStrings = new();
            testStrings[CasingStyle.Upper] = "THIS IS A TEST";
            testStrings[CasingStyle.Lower] = "this is a test";
            testStrings[CasingStyle.Pascal] = "ThisIsATest";
            testStrings[CasingStyle.Camel] = "thisIsATest";
            testStrings[CasingStyle.Kebab] = "this-is-a-test";
            testStrings[CasingStyle.Snake] = "this_is_a_test";
            testStrings[CasingStyle.Cobol] = "THIS-IS-A-TEST";
            testStrings[CasingStyle.Macro] = "THIS_IS_A_TEST";
            testStrings[CasingStyle.Train] = "This-Is-A-Test";
            testStrings[CasingStyle.Spreadsheet] = "This Is A Test";
            testStrings[CasingStyle.Mixed] = "this-Is_aTest";
            // read each casing into the system format
            foreach (KeyValuePair<CasingStyle, string> kvp in testStrings)
            {
                using (StringReader identifierReader = new(kvp.Value))
                {
                    Assert.AreEqual(testStrings[CasingStyle.Pascal], IdentifierCasing.CasingTranslator.ReadNormalizedCaseIdentifier(identifierReader));
                }
            }
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void CasingTranslatorOutputMixed()
        {
            IdentifierCasing.CasingTranslator.Translate("this-is-a-test", CasingStyle.Mixed);
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void CasingTranslatorWriteMixedCase()
        {
            StringBuilder written = new();
            using (StringWriter writer = new(written))
            {
                IdentifierCasing.CasingTranslator.WriteCasedIdentifier(writer, CasingStyle.Mixed, "ThisIsATest");
            }
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void CasingTranslatorEmptyIdentifier()
        {
            IdentifierCasing.CasingTranslator.DetectIdentifierCasing("");
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void CasingTranslatorNullIdentifier()
        {
            IdentifierCasing.CasingTranslator.DetectIdentifierCasing(null!);
        }
    }
}
