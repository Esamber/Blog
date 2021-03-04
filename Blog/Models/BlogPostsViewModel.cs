using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class BlogPostsViewModel
    {
        public List<Post> Posts { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
    public class ViewPostViewModel
    {
        public Post Post { get; set; }
        public string Name { get; set; }
    }
    public class MostRecentViewModel
    {
        public Post Post { get; set; }
    }
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime DatePosted { get; set; }
        public List<Comment> Comments { get; set; }
    }
    public class Comment
    {
        public string Name { get; set; }
        public string Body { get; set; }
        public DateTime DatePosted { get;set; }
    }
    public class BlogDb
    {
        private readonly string _connectionString;
        public BlogDb(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Post> GetPosts(int offset)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Posts
                ORDER BY Id desc
                OFFSET @offset ROWS FETCH
                NEXT 5 ROWS ONLY";
            cmd.Parameters.AddWithValue("@offset", offset);
            connection.Open();
            List<Post> posts = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                posts.Add(new Post
                {
                    Id = (int)reader["Id"],
                    Title = (string)reader["Title"],
                    Body = (string)reader["Body"],
                    DatePosted = (DateTime)reader["DatePosted"]
                });
            }
            return posts;
        }
        public void AddPost(Post p)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Posts (Title, Body, DatePosted) VALUES (@title, @body, @datePosted)";
            cmd.Parameters.AddWithValue("@title", p.Title);
            cmd.Parameters.AddWithValue("@body", p.Body);
            cmd.Parameters.AddWithValue("@datePosted", DateTime.Now);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public void AddComment(Comment c, int postId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Comments (PostId, Name, Body, DatePosted) VALUES (@postId, @name, @body, @datePosted)";
            cmd.Parameters.AddWithValue("@postId", postId);
            cmd.Parameters.AddWithValue("@name", c.Name);
            cmd.Parameters.AddWithValue("@body", c.Body);
            cmd.Parameters.AddWithValue("@datePosted", DateTime.Now);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public Post GetPost(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Posts where Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            Post post = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {

                post.Id = (int)reader["Id"];
                post.Title = (string)reader["Title"];
                post.Body = (string)reader["Body"];
                post.DatePosted = (DateTime)reader["DatePosted"];
                post.Comments = new List<Comment>();
            }
            return post;
        }
        public Post AddCommentsForPost(Post post)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Comments WHERE PostId = @id";
            cmd.Parameters.AddWithValue("@id", post.Id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                post.Comments.Add(new Comment
                {
                    Name = (string)reader["Name"],
                    Body = (string)reader["Body"],
                    DatePosted = (DateTime)reader["DatePosted"]
                });
            }
            return post;
        }
        public int GetMostRecentPostId()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT ISNULL(MAX(Id), -1) as MaxId FROM Posts";
            connection.Open();
            int maxId = (int)cmd.ExecuteScalar();
            return maxId;
        }
        public int GetTotalPages()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT COUNT(*) FROM Posts";
            connection.Open();
            int totalPosts = (int)cmd.ExecuteScalar();
            if (totalPosts % 5 == 0)
            {
                return (totalPosts/5);
            }
            return (totalPosts/5 + 1);
        }
    }
}
