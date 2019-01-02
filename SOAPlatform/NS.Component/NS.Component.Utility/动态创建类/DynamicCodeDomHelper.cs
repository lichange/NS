//using System;
//using System.CodeDom;
//using System.CodeDom.Compiler;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace NS.Component.Utility
//{
//    /// <summary>
//    /// 动态文件、程序集、方法等运行时创建的辅助类,都是在内存中完成的
//    /// </summary>
//    public class DynamicCodeDomHelper
//    {
//        #region 动态编译

//        public Assembly CompilerCode(IList<string> referencedAssemblies, string fileName)
//        {
//            CodeCompileUnit ccu = new CodeCompileUnit();
//            CodeDomProvider csc = CodeDomProvider.CreateProvider("CSharp");
//            CompilerParameters cplist = new CompilerParameters();
//            cplist.GenerateExecutable = false;
//            cplist.GenerateInMemory = true;

//            foreach (var item in referencedAssemblies)
//            {
//                cplist.ReferencedAssemblies.Add(item);
//            }

//            //读取文件内容
//            var str = FileOperate.ReadFile(fileName);
//            //结束文件读取

//            CompilerResults cr = csc.CompileAssemblyFromSource(cplist, str);
//            if (cr.Errors.Count > 0)
//            {
//                StringBuilder stringBuilder = new StringBuilder();
//                foreach (CompilerError item in cr.Errors)
//                {
//                    stringBuilder.Append("Error:" + item.ErrorText + "\r\n");
//                }
//                throw new Exception(stringBuilder.ToString());
//            }

//            Assembly loAssemble = cr.CompiledAssembly;

//            return loAssemble;
//        }

//        public Assembly CompilerCodeFromText(IList<string> referencedAssemblies, string codeText)
//        {
//            CodeCompileUnit ccu = new CodeCompileUnit();
//            CodeDomProvider csc = CodeDomProvider.CreateProvider("CSharp");
//            CompilerParameters cplist = new CompilerParameters();
//            cplist.GenerateExecutable = false;
//            cplist.GenerateInMemory = true;

//            foreach (var item in referencedAssemblies)
//            {
//                cplist.ReferencedAssemblies.Add(item);
//            }

//            CompilerResults cr = csc.CompileAssemblyFromSource(cplist, codeText);
//            if (cr.Errors.Count > 0)
//            {
//                StringBuilder stringBuilder = new StringBuilder();
//                foreach (CompilerError item in cr.Errors)
//                {
//                    stringBuilder.Append("Error:" + item.ErrorText + "\r\n");
//                }
//                throw new Exception(stringBuilder.ToString());
//            }

//            Assembly loAssemble = cr.CompiledAssembly;

//            return loAssemble;
//        }

//        public IList<Type> GetTypesFromDynamicAssembly(IList<string> referencedAssemblies, string codeText)
//        {
//            var assembly = this.CompilerCode(referencedAssemblies, codeText);
//            return assembly.GetTypes();
//        }


//        public Object Excute(IList<string> referencedAssemblies, string codeText, string className, string methodName, params object[] parameters)
//        {
//            var assembly = this.CompilerCode(referencedAssemblies, codeText);
//            var types = assembly.GetTypes();

//            var tempType = types.Where(pre => pre.Name == className).FirstOrDefault();
//            if (tempType == null)
//                throw new Exception("没有找到Type：" + className);

//            object typeInstance = Activator.CreateInstance(tempType, true);

//            MethodInfo[] methodInfos = tempType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//            if (methodInfos == null || methodInfos.Length == 0)
//                throw new Exception("没有找到方法：" + methodName);
//            MethodInfo method = tempType.GetMethod(methodInfos[0].Name.ToString());

//            try
//            {
//                var x = method.Invoke(typeInstance, parameters);

//                return x;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("方法调用失败，执行的过程中出现如下错误：" + ex.Message);
//            }
//        }

//        public CompilerResults CompilerCode(CodeCompileUnit unit, string language, CompilerParameters compilerParameters)
//        {
//            return CodeDomProvider.CreateProvider(language).CompileAssemblyFromDom(compilerParameters, unit);
//        }

//        public CompilerResults CompilerCode(CodeCompileUnit unit, string language, IList<string> referencedAssemblies, string outAssemblyName)
//        {
//            CompilerParameters option = new CompilerParameters();
//            option.GenerateExecutable = true;
//            option.GenerateInMemory = false;
//            option.IncludeDebugInformation = true;

//            foreach (var item in referencedAssemblies)
//            {
//                option.ReferencedAssemblies.Add(item);
//            }

//            option.OutputAssembly = outAssemblyName;
//            return CodeDomProvider.CreateProvider(language).CompileAssemblyFromDom(option, unit);
//        }

//        #endregion

//        #region 动态生成代码

//        public string GenerateCode(CodeCompileUnit unit, string language)
//        {
//            StringBuilder sb = new StringBuilder();
//            System.IO.StringWriter sw = new System.IO.StringWriter();
//            CodeDomProvider.CreateProvider(language).GenerateCodeFromCompileUnit
//            (unit, sw, null);
//            sw.Close();
//            return sw.ToString();
//        }

//        /// <summary>
//        /// 创建一个静态的main方法，作为程序的唯一入口，并且默认在该函数中加入console函数，输出test！
//        /// </summary>
//        /// <returns>CodeEntryPointMethod</returns>
//        public CodeEntryPointMethod CreateEntryPointMethodCode()
//        {
//            //Mehtod
//            CodeEntryPointMethod method = new CodeEntryPointMethod();
//            //Console.WriteLine("Hello Word!");
//            CodeMethodInvokeExpression methodWrite = new CodeMethodInvokeExpression(
//            new CodeTypeReferenceExpression(typeof(Console)), "WriteLine",
//            new CodePrimitiveExpression("test"));
//            //Console.Read();
//            CodeMethodInvokeExpression methodread = new CodeMethodInvokeExpression(
//            new CodeTypeReferenceExpression(typeof(Console)), "Read");
//            method.Statements.Add(methodWrite);
//            method.Statements.Add(methodread);

//            return method;
//        }

//        /// <summary>
//        /// 创建一个类的定义
//        /// </summary>
//        /// <param name="className">类名称</param>
//        /// <returns></returns>
//        public CodeTypeDeclaration CreateCodeTypeDeclaration(string className)
//        {
//            //class Hello
//            CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(className);
//            codeTypeDeclaration.Attributes = MemberAttributes.Public;

//            return codeTypeDeclaration;

//        }

//        /// <summary>
//        /// 创建一个命名空间
//        /// </summary>
//        /// <param name="typeNamespace">命名空间</param>
//        /// <param name="referencedAssemblies">引用的程序集空间名</param>
//        /// <returns></returns>
//        public CodeNamespace CreateCodeNamespace(string typeNamespace, IList<string> referencedAssemblies)
//        {
//            CodeNamespace nspace = new CodeNamespace(typeNamespace);

//            foreach (var item in referencedAssemblies)
//            {
//                nspace.Imports.Add(new CodeNamespaceImport(item));
//            }

//            return nspace;
//        }

//        /// <summary>
//        /// 创建属性字段field
//        /// </summary>
//        /// <param name="memberAttribute"></param>
//        /// <param name="fieldName"></param>
//        /// <param name="dataType"></param>
//        /// <returns></returns>
//        public CodeMemberField CreateCodeMemberField(MemberAttributes memberAttribute, string fieldName, Type dataType)
//        {
//            CodeMemberField tempValueField = new CodeMemberField();
//            tempValueField.Attributes = memberAttribute;
//            tempValueField.Name = fieldName;
//            tempValueField.Type = new CodeTypeReference(dataType);

//            return tempValueField;
//        }

//        /// <summary>
//        /// 创建构造函数
//        /// </summary>
//        /// <param name="fieldsDictionary">构造函数中的入参字典</param>
//        /// <returns></returns>
//        public CodeConstructor CreateCodeConstructor(Dictionary<Type, string> fieldsDictionary)
//        {
//            // Declare constructor
//            CodeConstructor constructor = new CodeConstructor();
//            constructor.Attributes =
//                MemberAttributes.Public | MemberAttributes.Final;

//            // Add parameters.
//            foreach (var kv in fieldsDictionary)
//            {
//                constructor.Parameters.Add(new CodeParameterDeclarationExpression(
//               kv.Key, kv.Value));
//            }

//            // Add field initialization logic
//            foreach (var kv in fieldsDictionary)
//            {
//                CodeFieldReferenceExpression reference =
//              new CodeFieldReferenceExpression(
//              new CodeThisReferenceExpression(), kv.Value);
//                constructor.Statements.Add(new CodeAssignStatement(reference,
//                            new CodeArgumentReferenceExpression(kv.Value)));
//            }

//            return constructor;
//        }

//        /// <summary>
//        /// 创建一个方法
//        /// </summary>
//        /// <param name="methodName">方法名</param>
//        /// <param name="paraDictionary">方法的形参清单</param>
//        /// <returns>返回CodeMemberMethod</returns>
//        public CodeMemberMethod CreateCodeMemberMethod(string methodName, Dictionary<Type, string> paraDictionary)
//        {
//            CodeMemberMethod tempMethod = new CodeMemberMethod();
//            tempMethod.Name = methodName;
//            tempMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
//            foreach (var item in paraDictionary)
//            {
//                tempMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(item.Key), item.Value));
//            }

//            return tempMethod;
//        }

//        /// <summary>
//        /// 创建一个EventHandler，调用后生成如下代码：
//        /// public event System.EventHandler eventName;
//        /// </summary>
//        /// <param name="eventName">事件参数名称</param>
//        /// <returns></returns>
//        public CodeMemberEvent CreateCodeMemberEvent(string eventName)
//        {
//            //field event
//            CodeMemberEvent tempEvent = new CodeMemberEvent();
//            tempEvent.Attributes = MemberAttributes.Public;
//            tempEvent.Type = new CodeTypeReference(typeof(EventHandler));
//            tempEvent.Name = eventName;
//            return tempEvent;
//        }

//        public CodeCompileUnit CodeDomHelloDemo()
//        {
//            //Mehtod
//            var method = this.CreateEntryPointMethodCode();
//            var codetype = this.CreateCodeTypeDeclaration("HelloWord");
//            codetype.Members.Add(method);

//            var nspace = this.CreateCodeNamespace("Demo", new string[] { "System.dll" });
//            nspace.Types.Add(codetype);

//            CodeCompileUnit unit = new CodeCompileUnit();
//            unit.Namespaces.Add(nspace);
//            return unit;
//        }

//        #endregion
//    }

//    /// <summary>
//    /// 一个Demo，基于CodeDOM自动生成
//    /// </summary>
//    public class CodeDom2
//    {
//        public CodeDom2()
//        {
//        }

//        public void Test()
//        {
//            char[] ch = "+ - + - + + - -".ToCharArray();
//            FuHaoSanJiao(ch, 0);
//        }

//        public void FuHaoSanJiao(char[] ch, int start)
//        {
//            Console.WriteLine(new string(ch));
//            if (start >= ch.Length / 2)
//                return;

//            for (int i = start; i < ch.Length - start - 1; i = i + 2)
//            {
//                if (ch[i] == ch[i + 2])
//                {
//                    ch[i + 1] = '+';
//                }
//                else
//                {
//                    ch[i + 1] = '-';
//                }
//                ch[i] = ' ';

//            }
//            ch[ch.Length - start - 1] = ' ';
//            FuHaoSanJiao(ch, start + 1);
//        }
//        public CodeNamespace CreateNameSpace()
//        {
//            //Test
//            CodeMemberMethod test = new CodeMemberMethod();
//            test.Name = "Test";
//            test.Attributes = MemberAttributes.Public | MemberAttributes.Final;
//            test.Statements.Add(new CodeVariableDeclarationStatement(typeof(char[]), "ch", new CodeMethodInvokeExpression(new CodePrimitiveExpression("+ - + - + + - -"), "ToCharArray", new CodeExpression[] { })));
//            test.Statements.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "FuHaoSanJiao"), new CodeExpression[] { new CodeVariableReferenceExpression("ch"), new CodePrimitiveExpression(0) }));
//            test.Statements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("System.Console"), "Read"));

//            //FuHaoSanJiao
//            CodeMemberMethod fuHaoSanJiao = new CodeMemberMethod();
//            fuHaoSanJiao.Name = "FuHaoSanJiao";
//            fuHaoSanJiao.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(char[])), "ch"));
//            fuHaoSanJiao.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "start"));
//            fuHaoSanJiao.Statements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("System.Console"), "WriteLine", new CodeExpression[] { new CodeObjectCreateExpression(typeof(string), new CodeArgumentReferenceExpression("ch")) }));
//            fuHaoSanJiao.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("start"), CodeBinaryOperatorType.GreaterThanOrEqual,
//                (new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("ch"), "Length"),
//                     CodeBinaryOperatorType.Divide, new CodePrimitiveExpression(2)))),
//                     new CodeMethodReturnStatement()));
//            CodeBinaryOperatorExpression condition = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),
//                     CodeBinaryOperatorType.LessThan,
//                     new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("ch"), "Length"),
//                       CodeBinaryOperatorType.Subtract, new CodeArgumentReferenceExpression("start")), CodeBinaryOperatorType.Subtract,
//                       new CodePrimitiveExpression(1)));//for 条件 
//            CodeConditionStatement iterationBody = new CodeConditionStatement(new CodeBinaryOperatorExpression(
//                new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("ch"), new CodeVariableReferenceExpression("i")),
//                 CodeBinaryOperatorType.IdentityEquality, new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("ch"),
//                     new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(2)))),

//                     new CodeAssignStatement(
//                     new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("ch"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),
//                     new CodePrimitiveExpression('+')));
//            iterationBody.FalseStatements.Add(new CodeAssignStatement(
//                     new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("ch"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),
//                     new CodePrimitiveExpression('-')));
//            CodeAssignStatement iteerationbody2 = new CodeAssignStatement(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("ch"), new CodeVariableReferenceExpression("i")),
//                new CodePrimitiveExpression(' '));

//            fuHaoSanJiao.Statements.Add(new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(int), "i", new CodeArgumentReferenceExpression("start")), condition,
//                new CodeAssignStatement(new CodeVariableReferenceExpression("i"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"), CodeBinaryOperatorType.Add,
//                    new CodePrimitiveExpression(2))), new CodeStatement[] { iterationBody, iteerationbody2 }));
//            // ch[ch.Length - start - 1] = ' ';
//            fuHaoSanJiao.Statements.Add(new CodeAssignStatement(
//                new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("ch"), new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("ch"), "Length"),
//                     CodeBinaryOperatorType.Subtract, new CodeArgumentReferenceExpression("start")), CodeBinaryOperatorType.Subtract,
//                     new CodePrimitiveExpression(1))), new CodePrimitiveExpression(' ')));
//            //  FuHaoSanJiao(ch, start + 1);
//            fuHaoSanJiao.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "FuHaoSanJiao", new CodeArgumentReferenceExpression("ch"),
//                new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("start"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));

//            CodeTypeDeclaration codeDomDemo2 = new CodeTypeDeclaration("CodeDomDemo2");
//            codeDomDemo2.Members.Add(test);
//            codeDomDemo2.Members.Add(fuHaoSanJiao);
//            codeDomDemo2.Attributes = MemberAttributes.Public;
//            codeDomDemo2.Comments.Add(new CodeCommentStatement("this code is from CodeDom!"));
//            //codeDomDemo2.Members.AddRange();

//            CodeNamespace nspace = new CodeNamespace("CodeDomDemo2");
//            nspace.Imports.Add(new CodeNamespaceImport("System"));
//            nspace.Types.Add(codeDomDemo2);
//            return nspace;
//        }
//    }
//}
