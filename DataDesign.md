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
||||
##反馈信心拓展-质量问题
dbo.ZL_FeedbackExProblem||||
-----|-----|-----|-----|
字段名|数据类型|说明|备注
ProblemID|bigint|自增1流水号ID|主键
CodeString|nvarchar(100)|质量编码|
ProblemDetails|nvarchar(1000)|质量问题详细描述|
PicturePath|nvarchar(1000)|质量问题图片url|存储多张图片url时，以逗号隔开
OrderNo|nvarchar(50)|反馈单号|
||||
## 反馈信心拓展-产生原因
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





10.10.152.75
GHLPY



