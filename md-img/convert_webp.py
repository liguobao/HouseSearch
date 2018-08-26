import os
import subprocess
folder_path = "./"
for file_path in os.listdir(folder_path):
    sub_path = os.path.join(folder_path, file_path)
    if os.path.isfile(sub_path) and ("png" in sub_path or "jpg" in sub_path):
        convert_webp_sh = "cwebp %s -o %s" % (sub_path,
                                              sub_path.replace("png", "webp").replace("jpg", "webp"))
        convert_result = subprocess.call(convert_webp_sh, shell=True)
        if convert_webp_sh == 0:
            subprocess.call("rm %s" % sub_path, shell=True)
            print("convert %s success!" % sub_path)
