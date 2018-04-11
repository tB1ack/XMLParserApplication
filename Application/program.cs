using System;
using System.Xml;

namespace avinodeXMLParser
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var menuPath = args[0];
                var activeMenu = args[1];

                XmlDocument doc = new XmlDocument();
                doc.Load(menuPath);

                // This initializes the first list of menu items to be parsed, ignoring content not required for this application
                var menuItemPath = "//menu/item";
                XmlNodeList menuNodes = doc.SelectNodes(menuItemPath);

                // This calls the method to parse the menu
                iterateMenu(menuNodes, activeMenu);
            }
            catch (System.ArgumentException)
            {
                // This is for when an empty path to menu is provided
                if (args[0] != null)
                    Console.WriteLine("No path to menu provided!");
            }
            catch (System.IO.FileNotFoundException)
            {
                // This is for when the menu path is invalid
                if (args[0] != null)
                    Console.WriteLine("The following menu path could not be found: {0}", args[0]);
            }
            catch
            {
                // This is for when there aren't enough arguments or the arguments
                Console.WriteLine("Something went wrong. Please make sure to provide a valid path to menu and active menu!");
            }
        }

        private static void iterateMenu(XmlNodeList menuNodes, string activeMenu, int level = 0)
        {
            level++; // This is used to keep track of what level the menu is at for alignment purposes later

            // This iterates through the list of menu items and calls a method to print the menu to the screen and check if the menu has submenus for each one
            foreach (XmlNode menuNode in menuNodes)
            {
                printMenu(menuNode, activeMenu, level);
                checkForSubMenu(menuNode, activeMenu, level);
            }
        }

        private static void printMenu(XmlNode menuNode, string activeMenu, int level)
        {
            // This gets the menu name and path value to be printed to the console
            XmlNode nameNode = menuNode.SelectSingleNode("displayName");
            XmlElement pathElement = (XmlElement)menuNode.SelectSingleNode("path");
            var pathValue = pathElement.GetAttributeNode("value").InnerXml;

            var printMenu = "";

            // This uses a helper method to determine if the menu or one of it's submenus is active
            if (checkForActiveSubMenu(menuNode, activeMenu,
                level) || pathValue == activeMenu)
            {
                printMenu = getMenuLevel(level) + nameNode.InnerText + ", " + pathValue + " ACTIVE";
            }
            else
            {
                printMenu = getMenuLevel(level) + nameNode.InnerText + ", " + pathValue;
            }
            Console.WriteLine(printMenu);
        }

        private static string getMenuLevel(int level)
        {
            // This helper method increases the indent for each submenu 
            string menuLevel = "";
            for (int i = 1; i < level; i++)
            {
                menuLevel += "\t";
            }
            return menuLevel;
        }

        private static bool checkForActiveSubMenu(XmlNode menuNode, string activeMenu = null, int level = 0)
        {
            // This creates a flag that is used to determine if the current menu or submenu is active
            bool activeMenuFlag = false;
            for (int i = 0; i < menuNode.ChildNodes.Count; i++)
            {
                XmlNodeList nodeCheck = menuNode.ChildNodes[i].SelectNodes(".//item");
                for (int j = 0; j < nodeCheck.Count; j++)
                {
                    XmlElement pathElement = (XmlElement)nodeCheck[j].SelectSingleNode("path");
                    var pathValue = pathElement.GetAttributeNode("value").InnerXml;
                    if (pathValue == activeMenu)
                        activeMenuFlag = true;
                }
            }
            return activeMenuFlag;
        }

        private static void checkForSubMenu(XmlNode menuNode, string activeMenu, int level)
        {
            // This iterates the menu's child nodes to see if it contains a submenu
            for (int i = 0; i < menuNode.ChildNodes.Count; i++)
            {
                if (menuNode.ChildNodes[i].Name == "subMenu")
                {
                    // If it does then it calls a method to get the menu items
                    getMenu(menuNode.ChildNodes[i], activeMenu, level);
                }
            }
        }

        private static void getMenu(XmlNode menuNode, string activeMenu, int level)
        {
            // This creates a list of menu items to be parsed; again ignoring any content not required
            var menuItemPath = "./item";
            XmlNodeList menuNodes = menuNode.SelectNodes(menuItemPath);

            // Then it calls the method that iterates the menu items
            iterateMenu(menuNodes, activeMenu, level);
        }
    }
}
