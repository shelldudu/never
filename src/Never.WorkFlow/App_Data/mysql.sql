CREATE TABLE template (
	Id INT(11) NOT NULL AUTO_INCREMENT,
	Name CHAR(50) NOT NULL,
	Text TEXT NULL,
	CreateDate DATETIME NULL DEFAULT NULL,
	EditDate DATETIME NULL DEFAULT NULL,
	Version INT(11) NULL DEFAULT NULL,
	PRIMARY KEY (Id),
	UNIQUE INDEX UNQ_Name (Name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*���ű��Ľű�*/
CREATE TABLE task (
	Id INT(11) NOT NULL AUTO_INCREMENT,
	TaskId CHAR(36) NOT NULL,
	UserSign VARCHAR(80) NULL DEFAULT NULL,
	UserSignState INT(11) NULL DEFAULT '0',
	AttachState INT(11) NULL DEFAULT '0',
	CommandType VARCHAR(20) NULL DEFAULT NULL,
	Status TINYINT(4) NULL DEFAULT NULL,
	StartTime DATETIME NULL DEFAULT NULL,
	NextTime DATETIME NULL DEFAULT NULL,
	FinishTime DATETIME NULL DEFAULT NULL,
	StepCount INT(11) NULL DEFAULT NULL,
	NextStep INT(11) NULL DEFAULT NULL,
	ProcessPercent DECIMAL(18,2) NULL DEFAULT NULL,
	PushMessage TEXT NULL,
	CreateDate DATETIME NULL DEFAULT NULL,
	EditDate DATETIME NULL DEFAULT NULL,
	Version INT(11) NULL DEFAULT NULL,
	PRIMARY KEY (Id),
	UNIQUE INDEX UNQ_TaskId (TaskId),
	INDEX IDX_UserSign (UserSign),
	INDEX IDX_Status (Status, AttachState)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE task_node (
	Id INT(11) NOT NULL AUTO_INCREMENT,
	RowId CHAR(36) NOT NULL,
	TaskId CHAR(36) NULL DEFAULT NULL,
	OrderNo INT(11) NULL DEFAULT NULL,
	StepCount INT(11) NULL DEFAULT NULL,
	StartTime DATETIME NULL DEFAULT NULL,
	FinishTime DATETIME NULL DEFAULT NULL,
	FailTimes INT(11) NULL DEFAULT NULL,
	WaitTimes INT(11) NULL DEFAULT NULL,
	StepType TEXT NULL,
	StepCoordinationMode TINYINT(4) NULL DEFAULT NULL,
	ExecutingMessage TEXT NULL,
	ResultMessage TEXT NULL,
	Removed BIT(1) NULL DEFAULT NULL,
	CreateDate DATETIME NULL DEFAULT NULL,
	EditDate DATETIME NULL DEFAULT NULL,
	Version INT(11) NULL DEFAULT NULL,
	PRIMARY KEY (Id),
	UNIQUE INDEX UNQ_RowId (RowId),
	INDEX IDX_TaskId (TaskId),
	INDEX IDX_Removed (Removed)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE task_node_error (
	RowId CHAR(36) NULL DEFAULT NULL,
	CreateDate DATETIME NULL DEFAULT NULL,
	StepType TEXT NULL,
	FailException TEXT NULL,
	INDEX IDX_RowId (RowId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*һ�ű��Ľű�*/
CREATE TABLE task (
	Id INT(11) NOT NULL AUTO_INCREMENT,
	TaskId CHAR(36) NOT NULL,
	StartTime DATETIME NULL DEFAULT NULL,
	NextTime DATETIME NULL DEFAULT NULL,
	FinishTime DATETIME NULL DEFAULT NULL,
	Status TINYINT(4) NULL DEFAULT NULL,
	StepCount INT(11) NULL DEFAULT NULL,
	NextStep INT(11) NULL DEFAULT NULL,
	UserSign VARCHAR(80) NULL DEFAULT NULL,
	UserSignState INT(11) NULL DEFAULT '0',
	AttachState INT(11) NULL DEFAULT '0',
	CommandType VARCHAR(20) NULL DEFAULT NULL,
	ProcessPercent DECIMAL(18,2) NULL DEFAULT NULL,
	CreateDate DATETIME NULL DEFAULT NULL,
	EditDate DATETIME NULL DEFAULT NULL,
	Version INT(11) NULL DEFAULT NULL,
	FailTimes INT(11) NULL DEFAULT NULL,
	WaitTimes INT(11) NULL DEFAULT NULL,
	PushMessage TEXT NULL,
	StepType TEXT NULL,
	CoordinationMode TEXT NULL,
	ExecutingMessage TEXT NULL,
	ResultMessage TEXT NULL,
	PRIMARY KEY (Id),
	UNIQUE INDEX UNQ_TaskId (TaskId),
	INDEX IDX_UserSign (UserSign),
	INDEX IDX_Status (Status, AttachState)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;