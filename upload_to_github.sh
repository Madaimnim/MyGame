#!/bin/bash
cd /d/Practice/TowerDefenseGame

# 取得當前日期與時間（格式：YYYY-MM-DD HH:MM:SS）
current_time=$(date +"%Y-%m-%d %H:%M:%S")

# 先把變更加入暫存
git add -A   # -A 可同時包含修改、新增、刪除檔案

# 判斷是否有變更（工作區 & 暫存區）
if git diff --quiet && git diff --cached --quiet; then
    echo "沒有檔案變更，不會建立 commit。"
else
    # 有變更才建立 commit 並推送
    git commit -m "更新專案 - $current_time"
    git push origin main
    echo "已推送到遠端 main 分支。"
fi

# 顯示提示，等按鍵後自動結束
read -n 1 -s -r -p "按下任意鍵關閉..."
exit