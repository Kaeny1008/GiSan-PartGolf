set GLOBAL general_log='ON'
set GLOBAL general_log='OFF'
set global log_output='TABLE';
set global log_output='FILE';
SET GLOBAL slow_query_log = 'ON';
SET GLOBAL slow_query_log = 'OFF';

show global variables like '%log_output%';
show global variables like '%slow%';

-- Log 보기
select * FROM mysql.general_log order by event_time desc;
SELECT * From mysql.slow_log;

-- Log 삭제
TRUNCATE mysql.general_log;
TRUNCATE mysql.slow_log;

explain
select * from tb_mms_material_basic_inventory tmmbi 
						       
						       
select * from tb_mmps_history_check


create index idx_clearance_date on tb_mms_material_basic_inventory(clearance_date)
drop index idx_clearance_date on tb_mms_material_basic_inventory

OPTIMIZE TABLE 
yj_mms.tb_mms_material_basic_inventory

explain

show binary logs;