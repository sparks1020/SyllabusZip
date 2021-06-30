/*
{
  "results": [
    {
      "id": "string",
      "parentId": "string",
      "title": "string",
      "body": "<!-- {\"bbMLEditorVersion\":1} --><div data-bbid=\"bbml-editor-id_9c6a9556-80a5-496c-b10d-af2a9ab22d45\"> <h4>Header Large</h4>  <h5>Header Medium</h5>  <h6>Header Small</h6>  <p><strong>Bold&nbsp;</strong><em>Italic&nbsp;<span style=\"text-decoration: underline;\">Italic Underline</span></em></p> <ul>   <li><span style=\"text-decoration: underline;\"><em></em></span>Bullet 1</li>  <li>Bullet 2</li> </ul> <p>  <img src=\"@X@EmbeddedFile.requestUrlStub@X@bbcswebdav/xid-1217_1\">   <span>\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\"</span> </p>  <p><span>&lt;braces test=\"values\" other=\"strange things\"&gt;</span></p> <p>Header Small</p> <ol>   <li>Number 1</li>   <li>Number 2</li> </ol>  <p>Just words followed by a formula</p>  <p><img align=\"middle\" alt=\"3 divided by 4 2 root of 7\" class=\"Wirisformula\" src=\"@X@EmbeddedFile.requestUrlStub@X@sessions/EA5F7FF3DF32D271D0E54AF0150D924A/anonymous/wiris/49728c9f5b4091622e2f4d183d857d35.png\" data-mathml=\"«math xmlns=¨http://www.w3.org/1998/Math/MathML¨»«mn»3«/mn»«mo»/«/mo»«mn»4«/mn»«mroot»«mn»7«/mn»«mn»2«/mn»«/mroot»«/math»\"></p> <p><a href=\"http://www.blackboard.com\">Blackboard</a></p> </div>",
      "description": "string",
      "created": "2021-05-02T14:45:42.207Z",
      "modified": "2021-05-02T14:45:42.207Z",
      "position": 0,
      "hasChildren": true,
      "hasGradebookColumns": true,
      "hasAssociatedGroups": true,
      "launchInNewWindow": true,
      "reviewable": true,
      "availability": {
        "available": "Yes",
        "allowGuests": true,
        "adaptiveRelease": {
          "start": "2021-05-02T14:45:42.208Z",
          "end": "2021-05-02T14:45:42.208Z"
        }
      },
      "contentHandler": {
        "id": "string"
      },
      "links": [
        {
          "href": "string",
          "rel": "string",
          "title": "string",
          "type": "string"
        }
      ]
    }
  ],
  "paging": {
    "nextPage": "string"
  }
}
*/

using System.Collections.Generic;

namespace SyllabusZip.Connectors.Blackboard.Models
{
    public class CourseContentsResults : ResultsList<Content>
    {
    }

    public class Content
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Description { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
        public int Position { get; set; }
        public bool HasChildren { get; set; }
        public bool HasGradebookColumns { get; set; }
        public bool HasAssociatedGroups { get; set; }
        public bool LaunchInNewWindow { get; set; }
        public bool Reviewable { get; set; }
        public ContentAvailability Availability { get; set; }
        public ContentHandler ContentHandler { get; set; }
        public IList<Link> Links { get; set; }
    }

    public class ContentAvailability
    {
        public string Available { get; set; }
        public bool AllowGuests { get; set; }
        // TODO: Add AdaptiveRelease if necessary
    }

    public class ContentHandler
    {
        public const string Document = "resource/x-bb-document";
        public const string ExternalLink = "resource/x-bb-externallink";
        public const string Folder = "resource/x-bb-folder";
        public const string CourseLink = "resource/x-bb-courselink";
        public const string ForumLink = "resource/x-bb-forumlink";
        public const string BltiLink = "resource/x-bb-blti-link";
        public const string File = "resource/x-bb-file";
        public const string TestLink = "resource/x-bb-asmt-test-link";
        public const string Assignment = "resource/x-bb-assignment";

        // Undocumented
        public const string Syllabus = "resource/x-bb-syllabus";

        public string Id { get; set; }

        // Extra metadata for certain types
        public string GradeColumnId { get; set; }
    }

    public class Link
    {
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
    }
}