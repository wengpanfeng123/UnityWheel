import os
import sys
import time
 
# 设置你本地的Unity安装目录
unity_exe = 'C:/Program Files/Unity/Hub/Editor/2022.3.28f1c1/Editor/Unity.exe'
# unity工程目录，当前脚本放在unity工程根目录中
project_path = 'D:/workspace/UnityProjects/MyDemo/Client'
# 日志
log_file = os.getcwd() + '/unity_log.log'
 
static_func = 'BuildTools.BuildApk'
 
# 杀掉unity进程
def kill_unity():
    os.system('taskkill /IM Unity.exe /F')
 
def clear_log():
    if os.path.exists(log_file):
        os.remove(log_file)
 
# 调用unity中我们封装的静态函数
def call_unity_static_func(func):
    kill_unity()
    time.sleep(1)
    clear_log()
    time.sleep(1)
    cmd = 'start %s -quit -batchmode -projectPath %s -logFile %s -executeMethod %s --productName:%s --version:%s'%(unity_exe,project_path,log_file,func, sys.argv[1], sys.argv[2])
    print('run cmd:  ' + cmd)
    os.system(cmd)
 
    
 
# 实时监测unity的log, 参数target_log是我们要监测的目标log, 如果检测到了, 则跳出while循环    
def monitor_unity_log(target_log):
    pos = 0
    while True:
        if os.path.exists(log_file):
            break
        else:
            time.sleep(0.1) 
    while True:
        fd = open(log_file, 'r', encoding='utf-8')
        if 0 != pos:
            fd.seek(pos, 0)
        while True:
            line = fd.readline()
            pos = pos + len(line)
            if target_log in line:
                print(u'监测到unity输出了目标log: ' + target_log)
                fd.close()
                return
            if line.strip():
                print(line)
            else:
                break
        fd.close()
 
if __name__ == '__main__':
    call_unity_static_func(static_func)
    monitor_unity_log('Build App Done!')
    print('done')
