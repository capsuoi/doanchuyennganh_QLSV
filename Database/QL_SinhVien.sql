USE master
GO

CREATE DATABASE Ql_SinhVien
GO

USE Ql_SinhVien
GO

CREATE TABLE nguoidung
(
	manguoidung int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	hoten nvarchar(255) NOT NULL,
	tendangnhap varchar(255) NOT NULL UNIQUE,
	email varchar(255) NOT NULL,
	matkhau varchar(255) NOT NULL,
	lahocsinh bit NOT NULL DEFAULT 1,
	bikhoa bit NOT NULL DEFAULT 0,
	tenlop nvarchar(255),
	cccd varchar(255),
	gioitinh bit NOT NULL DEFAULT 1,
	ngaysinh date NOT NULL,
	quocgia nvarchar(255),
	diachi nvarchar(255),
	sdt varchar(255),
	quoctich nvarchar(255),
	chuyennganh nvarchar(255)
)
GO

CREATE TABLE hoatdong
(
	mahoatdong int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	ten nvarchar(255) NOT NULL,
	ngaybatdau date NOT NULL,
	ngayketthuc date NOT NULL,
	soluongtoida int DEFAULT NULL,
	mota nvarchar(2000)
)
GO

CREATE TABLE hocsinh_hoatdong
(
	mahs_hd int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	mahocsinh int NOT NULL FOREIGN KEY REFERENCES nguoidung(manguoidung),
	mahoatdong int NOT NULL FOREIGN KEY REFERENCES hoatdong(mahoatdong),
	ngaydangky date NOT NULL,
	dathamgia bit NOT NULL DEFAULT 0,
	ngaythamgia date,
)
GO

CREATE TABLE diemdanh
(
	madiemdanh int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	mahocsinh_hoatdong  int NOT NULL REFERENCES hocsinh_hoatdong(mahs_hd),
	ngaydiemdanh date NOT NULL,
	comat bit NOT NULL DEFAULT 0,
	denmuon bit NOT NULL DEFAULT 0
)
GO

CREATE TRIGGER dangky_hoatdong_trigger ON hocsinh_hoatdong
INSTEAD OF INSERT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM inserted i WHERE EXISTS (SELECT 1 FROM hocsinh_hoatdong ue WHERE ue.mahocsinh = i.mahocsinh AND ue.mahoatdong = i.mahoatdong))
    BEGIN
        INSERT INTO hocsinh_hoatdong (mahocsinh, mahoatdong, ngaydangky, dathamgia, ngaythamgia)
        SELECT mahocsinh, mahoatdong, ngaydangky, dathamgia, ngaythamgia
        FROM inserted;
    END
END
GO

INSERT INTO nguoidung(tendangnhap, hoten, email, matkhau,  lahocsinh, tenlop, cccd, gioitinh, ngaysinh, quocgia, diachi, sdt, quoctich, chuyennganh) VALUES
('GV45904144', N'Nguyễn Thu Thủy', 'thuynt@gmail.com','29d601b7a5f8b4bd1581aa8210c6a338', 0, N'KH49A1', '00864242103', 0, '1989-12-11','VN','TP.HCM','033456789','vn', N'Lập trình viên quốc tế'),
('GV15218882', N'Alex Andree', 'alex.andree@gmail.com','b75bd008d5fecb1f50cf026532e8ae67', 0, N'KH50A1', '1185359998', 1, '1992-06-01','USA','New York','093456789','usa', N'Lập trình viên quốc tế'),
('SV87441102', N'Ngô Đức Anh', 'ngo.ducanh@gmail.com','22c4a6a464045fd8184563f5c301a95b', 1, N'KH49A1', '0059556954', 1, '2003-12-11','VN','TP.HCM','063726132','vn', N'Lập trình viên quốc tế'),
('SV27316502', N'Trần Duy Khoát', 'tran.duykhoat@gmail.com','d8b33e0838c4585d994cfc76d702ccf3', 1, N'KH49A1', '0081726245', 1, '2002-12-11','VN','TP.HCM','0785261322','vn', N'Lập trình viên quốc tế'),
('SV38846832', N'Phạm Tuấn Khải', 'pham.tuankhai@gmail.com','f8ad71cb6f452ee526d9c4c4f2cf7af4', 1, N'KH50A1', '0068091858', 1, '2004-12-11','VN','TP.HCM','0856127231','vn', N'Lập trình viên quốc tế'),
('SV66237549', N'Hạ Minh Vũ', 'ha.minhvu@gmail.com','4d1b1360997adfe21b9de68412d18288', 1, N'KH50A1', '0079269364', 1, '2003-12-11','VN','TP.HCM','0871273644','vn', N'Lập trình viên quốc tế'),
('SV50940890', N'Lê Xuân Đức', 'le.xuanduc@gmail.com','b033424a6a7d5e6dd1a134144f804e10', 1, N'KH50A1', '0071097928', 1, '2004-12-11','VN','TP.HCM','0957261232','vn', N'Lập trình viên quốc tế')
GO

INSERT INTO hoatdong(ten, ngaybatdau, ngayketthuc, mota) VALUES
(N'Thiết kế cơ sở dữ liệu', '2023-10-09', '2023-11-14', N'Tìm hiểu, phân tích các chức năng, yêu cầu của hệ thống từ đó đưa ra các đối tượng tham gia và xây dựng, phát triển cơ sở dữ liệu cho hệ thống với SQL SERVER!'),
(N'Lập trình ứng dụng web với ASPNET', '2023-11-15', '2024-02-09', N'Làm quen và tìm hiểu về C#, ASPNET. Học và xây dựng ứng dụng bằng C#, ASPNET theo mô hình MVC kết hợp OOP!')
GO

INSERT INTO hocsinh_hoatdong(mahocsinh, mahoatdong, ngaydangky, dathamgia, ngaythamgia) VALUES
(3, 1, '2023-09-22', 1, '2023-10-02'),
(4, 1, '2023-09-23', 1, '2023-10-02'),
(5, 1, '2023-09-25', 1, '2023-10-02'),
(6, 1, '2023-09-28', 1, '2023-10-02'),
(7, 1, '2023-09-24', 1, '2023-10-02'),
(3, 2, '2023-11-05', 0, NULL),
(4, 2, '2023-11-04', 0, NULL),
(5, 2, '2023-11-03', 0, NULL),
(6, 2, '2023-11-05', 0, NULL),
(7, 2, '2023-11-03', 0, NULL)
GO

INSERT INTO diemdanh(mahocsinh_hoatdong, ngaydiemdanh, comat, denmuon) VALUES
(1, '2023-10-09', 1, 0),(2, '2023-10-09', 1, 0),(3, '2023-10-09', 0, 0),(4, '2023-10-09', 1, 0),(5, '2023-10-09', 1, 0),
(1, '2023-10-14', 1, 0),(2, '2023-10-14', 1, 0),(3, '2023-10-14', 1, 0),(4, '2023-10-14', 1, 1),(5, '2023-10-14', 1, 0),
(1, '2023-10-16', 1, 0),(2, '2023-10-16', 1, 0),(3, '2023-10-16', 0, 0),(4, '2023-10-16', 1, 0),(5, '2023-10-16', 1, 0),
(1, '2023-10-21', 1, 0),(2, '2023-10-21', 1, 1),(3, '2023-10-21', 0, 0),(4, '2023-10-21', 0, 0),(5, '2023-10-21', 1, 0),
(1, '2023-10-23', 0, 0),(2, '2023-10-23', 1, 0),(3, '2023-10-23', 0, 0),(4, '2023-10-23', 1, 0),(5, '2023-10-23', 1, 0),
(1, '2023-10-28', 1, 0),(2, '2023-10-28', 1, 0),(3, '2023-10-28', 0, 0),(4, '2023-10-28', 1, 0),(5, '2023-10-28', 1, 0),
(1, '2023-10-30', 1, 0),(2, '2023-10-30', 1, 0),(3, '2023-10-30', 0, 0),(4, '2023-10-30', 1, 0),(5, '2023-10-30', 1, 0),
(1, '2023-11-05', 1, 0),(2, '2023-11-05', 0, 0),(3, '2023-11-05', 0, 0),(4, '2023-11-05', 0, 0),(5, '2023-11-05', 1, 1),
(1, '2023-11-07', 1, 0),(2, '2023-11-07', 1, 0),(3, '2023-11-07', 0, 0),(4, '2023-11-07', 1, 0),(5, '2023-11-07', 1, 0),
(1, '2023-11-12', 1, 0),(2, '2023-11-12', 1, 0),(3, '2023-11-12', 0, 0),(4, '2023-11-12', 1, 0),(5, '2023-11-12', 1, 0),
(1, '2023-11-14', 1, 1),(2, '2023-11-14', 1, 0),(3, '2023-11-14', 0, 1),(4, '2023-11-14', 1, 0),(5, '2023-11-14', 1, 0)
GO

SELECT * FROM nguoidung
SELECT * FROM hoatdong
SELECT * FROM hocsinh_hoatdong
SELECT * FROM diemdanh