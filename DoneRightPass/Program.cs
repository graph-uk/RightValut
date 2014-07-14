using System;
using System.Windows.Forms;
using DoneRightPass.Model;
using NHibernate;

namespace DoneRightPass
{
	internal static class Program
	{
		/// <summary>
		///     The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			SessionFactoryBuilder.BuildSessionFactory();

			ISession session = SessionFactoryBuilder.BuildSessionFactory().OpenSession();
			session.Save(new RecordType {Description = "blablabla", Id = 123, Name = "one"});
			session.Flush();
			session.Close();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}