//****************************
//* ba 工具，用于档案加工
//****************************


BlankApp.Cli : 命令行工具，新建档案目录等基本操作。
BlankApp.Configuration : 总体配置文件，开发时修改 'App.config'，发布运行时修改 'BlankApp.Configuration.dll.config'
BlankApp.ExcelAddIn : Excel插件适用于Excel2013以上版本，可快速生成修改数据库的sql代码
BlankApp.Input : UI版工具，内部调用 BlankApp.Cli，所以Cli,Input需要单独编译。
BlankApp.Service : 总体服务工具，服务与前4个项目。



