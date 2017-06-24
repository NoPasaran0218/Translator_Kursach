namespace Translator.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class TranslatorContext : DbContext
    {
        public TranslatorContext()
            : base("name=TranslatorContext")
        {
            Database.SetInitializer(new TranslatorInitializer());
        }

        public virtual DbSet<Compatible> Compatible { get; set; }
        public virtual DbSet<Lexem> Lexem { get; set; }
        public virtual DbSet<Const> Const { get; set; }
        public virtual DbSet<Identify> Identify { get; set; }
        public virtual DbSet<OutLexem> OutLexem { get; set; }
        public virtual DbSet<Automatic> Automatic { get; set; }

    }
    
    public class TranslatorInitializer:DropCreateDatabaseIfModelChanges<TranslatorContext>
    { 
        protected override void Seed(TranslatorContext context)
        {
            context.Lexem.Add(new Lexem() { Token = "prog", Separator = false});
            context.Lexem.Add(new Lexem() { Token = "var", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "int", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "double", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "cout", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "cin", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "if", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "then", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "do", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "while", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "enddo", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "{", Separator = true });
            context.Lexem.Add(new Lexem() { Token = "}", Separator = true });
            context.Lexem.Add(new Lexem() { Token = ",", Separator = true });
            context.Lexem.Add(new Lexem() { Token = "�", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "+", Separator = true });
            context.Lexem.Add(new Lexem() { Token = "-", Separator = true });
            context.Lexem.Add(new Lexem() { Token = "*", Separator = true });
            context.Lexem.Add(new Lexem() { Token = "/", Separator = true });
            context.Lexem.Add(new Lexem() { Token = "(", Separator = true });
            context.Lexem.Add(new Lexem() { Token = ")", Separator = true });


            Lexem lex1 = new Lexem() { Token = "=", Separator = true };
            context.Lexem.Add(lex1);
            context.SaveChanges();
            Compatible compatible1 = new Compatible() { CanJoin = "=", Lexem = lex1 };
            context.Compatible.Add(compatible1);
            context.SaveChanges();


            context.Lexem.Add(new Lexem() { Token = "<<", Separator = false });
            context.Lexem.Add(new Lexem() { Token = ">>", Separator = false });

            lex1 = new Lexem() { Token = "<", Separator = true };
            context.Lexem.Add(lex1);
            context.SaveChanges();
            compatible1 = new Compatible() { CanJoin = "<", Lexem = lex1 };
            Compatible compatible2 = new Compatible() { CanJoin = "=", Lexem = lex1 };
            context.Compatible.Add(compatible1);
            context.Compatible.Add(compatible2);
            context.SaveChanges();


            lex1 = new Lexem() { Token = ">", Separator = true };
            context.Lexem.Add(lex1);
            context.SaveChanges();
            compatible1 = new Compatible() { CanJoin = ">", Lexem = lex1 };
            compatible2 = new Compatible() { CanJoin = "=", Lexem = lex1 };
            context.Compatible.Add(compatible1);
            context.Compatible.Add(compatible2);
            context.SaveChanges();

            context.Lexem.Add(new Lexem() { Token = "<=", Separator = false });
            context.Lexem.Add(new Lexem() { Token = ">=", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "==", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "!=", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "and", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "or", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "not", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "idn", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "const", Separator = false });
            context.Lexem.Add(new Lexem() { Token = "[", Separator = true });
            context.Lexem.Add(new Lexem() { Token = "]", Separator = true });
            context.SaveChanges();


            context.Automatic.Add(new Automatic() { A = 1, Token = "prog", B = 2, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 1, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 2, Token = "idn", B = 3, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 1, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 3, Token = "var", B = 10, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 3, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 10, Token = "int", B = 11, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 10, Token = "double", B = 11, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 10, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 11, Token = "idn", B = 4, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 11, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 4, Token = ",", B = 11, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 4, Token = "�", B = 5, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 4, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 5, Token = "int", B = 11, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 5, Token = "double", B = 11, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 5, Token = "{", B = 100, Stack = 6, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 5, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 6, Token = "�", B = 7, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 6, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 7, Token = "}", B = null, Stack = null, AdditionInform = "[=] exit" });
            context.Automatic.Add(new Automatic() { A = 7, Token = "[!=]", B = 100, Stack = 6, AdditionInform = "[!=] <p.a. operator>" });

            context.Automatic.Add(new Automatic() { A = 100, Token = "cout", B = 101, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 101, Token = "<<", B = 102, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 101, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 102, Token = "idn", B = 103, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 102, Token = "const", B = 103, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 102, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 103, Token = "<<", B = 102, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 103, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] exit" });

            context.Automatic.Add(new Automatic() { A = 100, Token = "cin", B = 104, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 104, Token = ">>", B = 105, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 104, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 105, Token = "idn", B = 106, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 105, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });


            context.Automatic.Add(new Automatic() { A = 106, Token = ">>", B = 105, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 106, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] exit" });

            context.Automatic.Add(new Automatic() { A = 100, Token = "idn", B = 107, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 107, Token = "=", B = 200, Stack = 108, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 107, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 108, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] exit" });

            context.Automatic.Add(new Automatic() { A = 100, Token = "do", B = 109, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 109, Token = "while", B = 110, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 109, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 110, Token = "(", B = 300, Stack = 111, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 110, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 111, Token = ")", B = 501, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 111, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 112, Token = "�", B = 113, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 112, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 113, Token = "enddo", B = null, Stack = null, AdditionInform = "[=] exit" });
            context.Automatic.Add(new Automatic() { A = 113, Token = "[!=]", B = 100, Stack = 112, AdditionInform = "[!=] <p.a. operator>" });

            context.Automatic.Add(new Automatic() { A = 100, Token = "if", B = 114, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 114, Token = "(", B = 300, Stack = 115, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 114, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });
            context.Automatic.Add(new Automatic() { A = 115, Token = ")", B = 116, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 115, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });
            context.Automatic.Add(new Automatic() { A = 116, Token = "then", B = 117, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 116, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });
            context.Automatic.Add(new Automatic() { A = 117, Token = "{", B = 100, Stack = 119, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 117, Token = "[!=]", B = 100, Stack = 118, AdditionInform = "[!=] <p.a. operator>" });
            context.Automatic.Add(new Automatic() { A = 118, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] exit" });
            context.Automatic.Add(new Automatic() { A = 119, Token = "�", B = 120, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 119, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });
            context.Automatic.Add(new Automatic() { A = 120, Token = "}", B = null, Stack = null, AdditionInform = "[=] exit" });
            context.Automatic.Add(new Automatic() { A = 120, Token = "[!=]", B = 100, Stack = 119, AdditionInform = "[!=] <p.a. operator>" });

            context.Automatic.Add(new Automatic() { A = 100, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] exit" });


            context.Automatic.Add(new Automatic() { A = 200, Token = "-", B = 200, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 200, Token = "(", B = 200, Stack = 202, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 200, Token = "idn", B = 201, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 200, Token = "const", B = 201, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 200, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 201, Token = "+", B = 200, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 201, Token = "-", B = 200, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 201, Token = "*", B = 200, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 201, Token = "/", B = 200, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 201, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] exit" });

            context.Automatic.Add(new Automatic() { A = 202, Token = ")", B = 201, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 202, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 300, Token = "not", B = 300, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 300, Token = "(", B = 300, Stack = 303, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 300, Token = "[!=]", B = 200, Stack = 301, AdditionInform = "[!=] <p.a. E>" });

            context.Automatic.Add(new Automatic() { A = 301, Token = ">", B = 200, Stack = 302, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 301, Token = "<", B = 200, Stack = 302, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 301, Token = ">=", B = 200, Stack = 302, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 301, Token = "<=", B = 200, Stack = 302, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 301, Token = "!=", B = 200, Stack = 302, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 301, Token = "==", B = 200, Stack = 302, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 301, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 302, Token = "and", B = 300, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 302, Token = "or", B = 300, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 302, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] exit" });

            context.Automatic.Add(new Automatic() { A = 303, Token = ")", B = 302, Stack = null, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 303, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });

            context.Automatic.Add(new Automatic() { A = 501, Token = "�", B = 100, Stack = 112, AdditionInform = "" });
            context.Automatic.Add(new Automatic() { A = 501, Token = "[!=]", B = null, Stack = null, AdditionInform = "[!=] error" });
            context.SaveChanges();

            base.Seed(context);
        }
    }

}