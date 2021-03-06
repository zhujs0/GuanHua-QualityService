# 质量管理系统设计
## 工序设计
dbo.ZL_Room||||
-----:|-----:|-----:|-----:|
字段名|数据类型|说明|备注
RoomID|bigint|自增1流水号|
RoomName|nvarchar(100)|工序名|主键，唯一
RoomCode|nvarchar(10)|工序代码|
||||

## 编码设计
dbo.ZL_QualityCode||||
-----|:-----:|-----:|-----:|
字段名|数据类型|说明|备注
CodeID|bigint|编码维护流水号|自增
CodeString|nvarchar(100)|编码|主键，唯一
RoomCode|nvarchar(10)|质量问题工序代码|not null,工序编码
RoomName|nvarchar(100)|质量问题工序|not null,工序名称
TypeName|nvarchar(100)|质量分类|
TypeCode|nvarchar(10)|质量分类编码|
Problem|nvarchar(100)|质量问题|
ProCode|nvarchar(10)|质量问题编码|
Present|nvarchar(100)|质量比例|
PreCode|nvarchar(10)|质量比例编码|
Suggestion|nvarchar(1000)|处理意见|存储方式：json数组形式
QualityClass|nvarchar(50)|质量判类|
|||||
## 基础反馈信息设计
dbo.ZL_FeedbackBase||||
-----|-----|-----|-----|
字段名|数据类型|说明|备注
OrderNo|nvarchar(50)|反馈单号|主键，唯一
WorkProcedure|nvarchar(100)|工序|对应dbo.Room中的RoomName
BatchNo|nvarchar(50)|批号|
Model|nvarchar(50)|型号规格|
Qty|nvarchar(50)|数量|
EquipmentName|nvarchar(50)|设备名|
EquipmentNo|nvarchar(50)|设备号|
FeedbackMan|nvarchar(50)|反馈人
FeedbackTime|datetime|反馈时间
HPrint|tinyint|是否打印|0表示未打印，1表示已打印
Status|tinyint|状态|0表示待审批，1表示已完成
ProblemLevel|nvarchar(50)|审批流程代号
ProductClass|nvarchar(50)|产品类别
||||
##反馈信息拓展-质量问题
dbo.ZL_FeedbackExProblem||||
-----|-----|-----|-----|
字段名|数据类型|说明|备注
ProblemID|bigint|自增1流水号ID|主键
CodeString|nvarchar(100)|质量编码|
ProblemDetails|nvarchar(1000)|质量问题详细描述|
PicturePath|nvarchar(1000)|质量问题图片url|存储多张图片url时，以逗号隔开
OrderNo|nvarchar(50)|反馈单号|
||||
## 反馈信息拓展-产生原因
dbo.ZL_FeedbackExReason||||
-----|-----|-----|-----|
字段名|数据类型|说明|备注
ReasonID|bigint|自增1流水号ID|主键
ReasonType|nvarchar(50)|原因类型|人机料法环
ReasonDetails|nvarchar(1000)|原因详细描述|
OrderNo|nvarchar(50)|反馈单号|
||||
## 反馈信心拓展-处理意见
dbo.ZL_FeedbackExHandle||||
-----|-----|-----|-----|
字段名|数据类型|说明|备注
HandleID|bigint|自增1流水号ID|主键
HandleMan|nvarchar(100)|处理人|
HandleSuggestion|nvarchar(1000)|处理意见|
HandleTime|datetime|处理时间|
OrderNo|nvarchar(50)|反馈单号|
<div style="color:#FFC125">HandleNote</div>|<div style="color:#FFC125">nvarchar(100)</div>|<div style="color:#FFC125"></div>|<div style="color:#FFC125">待确定，暂未使用字段</div>
QualityClass|nvarchar(50)|质量判类|
|||||

## 相关批号信息设计
dbo.ZL_ParentTable||||
-----|-----|-----|-----|
字段名|数据类型|说明|备注
ParentNo|nvarchar(50)|父批号|
ChildNo|nvarchar(50)|子批号|当前批号
|||||

## 人员设置
dbo.ZL_PersonSetup||||
-----|-----|-----|-----|
字段名|数据类型|说明|备注
PersonAutoID|bigint|自动流水号|主键
EmployeeNo|nvarchar(50)|工号|
EmployeeName|nvarchar(50)|职工姓名|
WorkProduct|nvarchar(50)|负责审核的工序
WorkName|nvarchar(50)|负责的岗位|例：测试QC啊，沉积QC，沉积工艺员，部门部
|||||

## 岗位设置
dbo.ZL_WorkSetup||||
-----|-----|-----|-----|
字段名|数据类型|说明|备注
WorkAutoID|bigint|自动流水号|主键
WorkProduct|nvarchar(50)|工序/部门|
WorkName|nvarchar(50)|负责的岗位|例：测试QC啊，沉积QC，沉积工艺员，部门部长，经理
|||||


## 审批
dbo.ZL_ApprovalStream||||
-----|-----|-----|-----|
字段名|数据类型|说明|备注
AutoID|bigint|自动流水号|主键
ManPosition|nvarchar(50)|职务(对应审批流程)|例：测试QC啊，沉积QC，沉积工艺员，部门部长，经理
Man|nvarchar(50)|审批人|
HandlingSuggestion|nvarchar(1000)|处理意见
ApprovalDate|datetime|审批时间
ToClass|nvarchar(50)|判类
OrderNo|nvarchar(50)|反馈单单号
|||||

## 反馈单-备货卡/发货卡
dbo.ZL_Card||||
-----|-----|-----|-----|
字段名|数据类型|说明|备注
CardAutoID|bigint|自动流水号|主键
FKOrderNo|nvarchar(50)|反馈单单号)|
CardNo|nvarchar(50)|卡号|
OrderNo|nvarchar(50)|订单单号
Customer|nvarchar(50)|客户名称
TempClass|nvarchar(50)|备货/客户类别
TempModel|nvarchar(50)|备货型号规格
ProductModel|nvarchar(50)|产品型号规格|
ProductClass|nvarchar(50)|产品类别
Amount|decimal(18, 2)|数量
TempAmount|decimal(18, 2)|备货数量
BatchNo|nvarchar(50)|批号
|||||





10.10.152.75
GHLPY



