# SLED
WCTC is required to report student learning evidence to the state, board, and others.

## Model Binding and View Models
Instead of using form controls you should bind a ViewModel to your controller method. Think of the view models as a shell of all the data you will need in the given View. I have put together a slideshow with a short explanation here:
- https://docs.google.com/presentation/d/1tBl486R-VpDw_TXrjX3NKXrhzyHfa3xBlQVnSn9wfOE/edit?usp=sharing

For more information on implementing this correctly research "model binding in .Net." and refer to the files mentioned in the presentation in the actual repository. 

**IMPORTANT** NOTE: These files may not be in the "master" branch check the "Admin_Group_branch."

## Requirements

* .NET Framework 4.6.1
* [Visual Studio IDE](https://visualstudio.microsoft.com/)

## Installation

## If you already have Visual Studio installed skip steps (1-4)
## Installing Visual Studio
1. Launch the Visual Studio Launcher
2. Under Windows select ".NET Desktop Development
3. Under Web & Cloud select ASP.NET and web development
4. Click Install in the bottom right corner
## Opening the project from GitHub
5. Open Visual Studio
6. Select View in the navigation and open the Team Explorer
7. Under local git repositories select the Clone option
8. If you're reading this you already have access to the GitHub repo. Select Clone or Download and copy the GitHub repo link
9. Paste the GitHub repo link in the Visual Studio Clone option
10. Onced cloned, double click on the repository, select Branches
11. Expand CLS-SLE, right click on master and select the pull option
12. Your local repo should now be updated to the most current commit
## Committing changes to GitHub
13. Open the Team Explorer (Step 6)
14. Select the Changes option
15. Give your changes a useful commit message and Commit All
16. **IMPORTANT** If your local repository version is behind the version committed to GitHub you will need Pull before Pushing
17. To pull changes repeat step 11
18. After the changes have been pulled, right click on master and select Push


