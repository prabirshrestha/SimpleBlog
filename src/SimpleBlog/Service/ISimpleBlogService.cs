﻿namespace SimpleBlog.Service
{
    using SimpleBlog.Models;

    public interface ISimpleBlogService
    {
        BlogModel GetBlog();

        string TransformContent(string input);
    }
}