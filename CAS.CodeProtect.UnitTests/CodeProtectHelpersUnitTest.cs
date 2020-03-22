
using UAOOI.CodeProtect.EnvironmentAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace UAOOI.CodeProtect
{
  [TestClass]
  public class CodeProtectHelpersUnitTest
  {
    [TestMethod]
    public void GetArgumentsCommandLineTest()
    {
      Assert.Inconclusive("Fakes doesn't work");
      string[] _arguments = CodeProtectHelpers.GetArguments();
      Assert.IsNotNull(_arguments);
      Assert.AreEqual<int>(3, _arguments.Length);
      Assert.IsTrue(_arguments[0].ToLower().Contains("c:\\program files (x86)\\microsoft"), _arguments[0].ToLower());
      Assert.IsTrue(_arguments[1].Contains(@"/parentProcessId"), _arguments[1].ToLower());
      Assert.IsTrue(int.Parse(_arguments[2]) > -1, _arguments[2]);
    }
    [TestMethod]
    public void GetArgumentsPredefinedTest()
    {
      Assert.Inconclusive("Fakes doesn't work");
      //using (ShimsContext.Create())
      //{
      //  bool _shimed = false;
      //  //ShimEnvironment.GetCommandLineArgs = () => { _shimed = true; return new string[] { "Lorem", "ipsum", "dolor" }; };
      //  string[] _arguments = CodeProtectHelpers.GetArguments();
      //  Assert.IsTrue(_shimed);
      //  Assert.IsNotNull(_arguments);
      //  Assert.AreEqual<int>(3, _arguments.Length);
      //  Assert.IsTrue(_arguments[0].Equals("Lorem"), _arguments[0]);
      //  Assert.IsTrue(_arguments[1].Equals("ipsum"), _arguments[1]);
      //  Assert.IsTrue(_arguments[2].Equals("dolor"), _arguments[2]);
      //}
    }
  }
}
