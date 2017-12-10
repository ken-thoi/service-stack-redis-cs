using System;
using ServiceStackRedis.ConsApp.BlogPortStore;

namespace ServiceStackRedis.ConsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //UserExamples.OnBeforeEachInvokeMeothod();
            //UserExamples.StoreAndRetrieveUsers();

            //UserAndBlogStoreMag.OnBeforeEachRun();
            //UserAndBlogStoreMag.StoreAndRetrieveSomeBlogs();

            BlogPostMag.OnBeforeEachRun();
            BlogPostMag.ShowListOfBlogs();
            BlogPostMag.ShowListOfRecentPostsAndComments();
            BlogPostMag.ShowATagCloud();
            BlogPostMag.ShowAllCategories();
            BlogPostMag.ShowPostAndAllComments();
            BlogPostMag.AddCommentToExistingPost();
            BlogPostMag.ShowAllPostsForTheDocumentDbCategory();

            Console.ReadKey();
        }
    }
}
