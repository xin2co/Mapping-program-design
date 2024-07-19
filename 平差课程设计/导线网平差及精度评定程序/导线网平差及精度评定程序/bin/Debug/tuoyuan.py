# -*- coding: utf-8 -*-

import matplotlib.pyplot as plt
import numpy as np
import math

plt.rcParams['font.sans-serif'] = ['SimHei']     
plt.rcParams['axes.unicode_minus'] = False

# 定义参数
doubleValue41, doubleValue42, doubleValue43 = 384.317, 555.521, 82.468
doubleValue51, doubleValue52, doubleValue53 = 644.094, 205.958, -97.379
seigemaquan = 13.5
point1 = (122868.443, 253698.946)
point2 = (126886.487, 258138.106)

# 定义 calculate_ellipse_parameters 函数
def calculate_ellipse_parameters(Qxx, Qyy, Qxy, seigemaquan):
    K = math.sqrt((Qxx - Qyy) * (Qxx - Qyy) + 4 * Qxy * Qxy)
    QEE = 0.5 * (Qxx + Qyy + K)
    QFF = 0.5 * (Qxx + Qyy - K)
    PhiE = math.atan((QEE - QFF) / Qxy)
    E = seigemaquan * math.sqrt(QEE)
    F = seigemaquan * math.sqrt(QFF)
    return E, F, PhiE

# 计算椭圆参数
E1, F1, PhiE1 = calculate_ellipse_parameters(doubleValue41, doubleValue42, doubleValue43, seigemaquan)
E2, F2, PhiE2 = calculate_ellipse_parameters(doubleValue51, doubleValue52, doubleValue53, seigemaquan)

# 创建一个新的图像
fig, ax = plt.subplots()

# 绘制椭圆  
t = np.linspace(0, 2 * np.pi, 100)
x1 = E1 * np.cos(t) * np.cos(PhiE1) - F1 * np.sin(t) * np.sin(PhiE1) + point1[0]
y1 = E1 * np.cos(t) * np.sin(PhiE1) + F1 * np.sin(t) * np.cos(PhiE1) + point1[1]
x2 = E2 * np.cos(t) * np.cos(PhiE2) - F2 * np.sin(t) * np.sin(PhiE2) + point2[0]
y2 = E2 * np.cos(t) * np.sin(PhiE2) + F2 * np.sin(t) * np.cos(PhiE2) + point2[1]

ax.plot(x1, y1, label="4号点误差椭圆")
ax.plot(x2, y2, label="5号点误差椭圆")

# 设置坐标轴比例和标题
ax.set_aspect('equal')
ax.set_title('俩椭圆')
ax.legend()

# 显示图像
plt.show()
