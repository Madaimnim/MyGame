@echo off
git add .
git commit -m "更新專案" >nul 2>&1
git push origin main >nul 2>&1
exit