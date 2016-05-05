using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Frame.DataStore;
using Frame.DataStore.SqlGeClient;

namespace Frame.Test.Web.DaoTest
{
    public partial class SqlGeStatementPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnTestStatement1_Click(object sender, EventArgs e)
        {
            string sql = "select * from users where id = #user_id#";
            IDictionary<string, object> parameters = new Dictionary<string, object>(){{"user_id",1}};
            SqlGeStatement statement = SqlGeParser.Parse(sql) as SqlGeStatement;
            ISqlGeCommand command = statement.CreateCommand(parameters);

            // 转换为 select * from users where id = @user_id
            string text = command.CommandText;

            sql = "select * from users where id = @user_id";
            parameters = new Dictionary<string, object>(){{"user_id",1}};
            statement = SqlGeParser.Parse(sql) as SqlGeStatement;
            command = statement.CreateCommand(parameters);

            // select * from users where id = @user_id
            text = command.CommandText;

        }

        protected void btnTestStatement2_Click(object sender, EventArgs e)
        {
            string sql = "select * from users where id = $user_id$";
            IDictionary<string, object> parameters = new Dictionary<string, object>(){{"user_id",1}};
            SqlGeStatement statement = SqlGeParser.Parse(sql) as SqlGeStatement;
            ISqlGeCommand command = statement.CreateCommand(parameters);
            // 转换为 select * from users where id = 1
            string text = command.CommandText;

            sql = "select * from users where id = $user_id?20$";
            parameters = new Dictionary<string, object>(){{"user_id",1}};
            statement = SqlGeParser.Parse(sql) as SqlGeStatement;
            command = statement.CreateCommand(parameters);
            // 转换为 select * from users where id = 1
            text = command.CommandText;


            sql = "select * from users where id = $user_id?20$";            
            statement = SqlGeParser.Parse(sql) as SqlGeStatement;
            command = statement.CreateCommand(null);
            // 转换为 select * from users where id = 20
            text = command.CommandText;

            sql = "select * from users where id in ($user_id$)";
            parameters = new Dictionary<string, object>() { 
                { "user_id", new List<string>(){"1","2","3","4"} } 
            };
            statement = SqlGeParser.Parse(sql) as SqlGeStatement;
            command = statement.CreateCommand(parameters);
            // 转换为 select * from users where id in ('1','2','3','4')
            text = command.CommandText;

            sql = "select * from users where id in ($user_id$)";
            parameters = new Dictionary<string, object>() { 
                { "user_id", new List<int>(){1,2,3,4} } 
            };
            statement = SqlGeParser.Parse(sql) as SqlGeStatement;
            command = statement.CreateCommand(parameters);
            // 转换为 select * from users where id in (1,2,3,4)
            text = command.CommandText;
        }

        protected void btnTestStatement3_Click(object sender, EventArgs e)
        {
            string sql = "select * from users where id = #user_id# {? and name = '$user_name$'}";
            IDictionary<string, object> parameters = new Dictionary<string, object>(){
                {"user_id",1},
                {"user_name","xiaoming"}
            };
            SqlGeStatement statement = SqlGeParser.Parse(sql) as SqlGeStatement;
            ISqlGeCommand command = statement.CreateCommand(parameters);
            // 转换为 select * from users where id = @user_id and name = 'xiaoming'
            string text = command.CommandText;


            parameters = new Dictionary<string, object>(){{"user_id",1}};
            command = statement.CreateCommand(parameters);
            // 转换为 select * from users where id = @user_id 
            text = command.CommandText;
        }
    }
}