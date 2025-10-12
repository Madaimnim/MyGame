#!/bin/bash
cd /d/Practice/TowerDefenseGame

# 取得當前日期與時間（格式：YYYY-MM-DD HH:MM:SS）
current_time=$(date +"%Y-%m-%d %H:%M:%S")


echo "🔄 先從遠端下載最新版本 (git pull)..."
git pull origin main

# 檢查 pull 是否成功
if [ $? -ne 0 ]; then
    echo "⚠️ 下載最新版本失敗，請先解決衝突或檢查網路。"
    read -n 1 -s -r -p "按下任意鍵關閉..."
    exit 1
fi



# 先把變更加入暫存
git add -A   # -A 可同時包含修改、新增、刪除檔案

# 判斷是否有變更（工作區 & 暫存區）
if git diff --quiet && git diff --cached --quiet; then
    echo "沒有檔案變更，不會建立 commit。"
else
  # 允許輸入自訂 commit 訊息（預設：更新專案）
    read -p "請輸入 commit 訊息（預設：更新專案）：" user_msg
    if [[ -z "${user_msg// }" ]]; then
      user_msg="更新專案"
    fi

    # 有變更才建立 commit 並推送
    git commit -m "$user_msg - $current_time"

    # 推送
    git push origin main

    echo "已推送到遠端 main 分支。"

    echo "📌 最新 Commit："
    git log -1 --oneline --decorate
fi

# 顯示提示，等按鍵後自動結束視窗
read -n 1 -s -r -p "按下任意鍵關閉..."
exit