USE master
GO

/*==============================================================*/
/*====================== CREATE DATABASE =======================*/
/*==============================================================*/

CREATE DATABASE QL_TourDuLich
ON PRIMARY
(
	NAME = QL_TourDuLich_Primary,
	FILENAME = 'D:\Năm 3\Lap Trinh Wed\Tour Du Lich\SQL\primary_log\QL_TourDuLich_Primary.mdf',
	SIZE = 10MB,
	MAXSIZE = 100MB,
	FILEGROWTH = 10%
)
LOG ON
(
	NAME = QL_TourDuLich_Log,
	FILENAME = 'D:\Năm 3\Lap Trinh Wed\Tour Du Lich\SQL\primary_log\QL_TourDuLich_Log.ldf',
	SIZE = 10MB,
	MAXSIZE = 100MB,
	FILEGROWTH = 10%
)
GO

USE QL_TourDuLich
GO

/*==============================================================*/
/*======================== CREATE TABLE ========================*/
/*==============================================================*/

--CREATE TABLE KHACHSAN
--(
--	MaKS VARCHAR(10) PRIMARY KEY,
--	TenKS NVARCHAR(50) not null,
--	Diachi NVARCHAR(80) not null,
--	SDT VARCHAR(11) null,
--	ChiPhi FLOAT not null,
--)
--GO

CREATE TABLE DIEMTHAMQUAN
(
	MaDiaDanh VARCHAR(10) PRIMARY KEY,
	TenDiaDanh NVARCHAR(80) not null,
	Diachi NVARCHAR(50) not null,
	HinhAnh NVARCHAR(50) null default N'Không có hình ảnh',
	DacTrung NVARCHAR(50) default N'Không có thông tin',
	ChiPhi decimal(19,2) not null
	--MaKS VARCHAR(10) default N'Không Có Khách Sạn',
)
GO

--CREATE TABLE PhuongTien
--(
--	MaPT VARCHAR(10) not null,
--	TenPT NVARCHAR(50) not null,
--	LoaiPT NVARCHAR(50) not null,
--	SoCho INT DEFAULT 45,
--	MaTour VARCHAR(10) not null,
--	CONSTRAINT pk_PhuongTien PRIMARY KEY (MaPT)
--)

CREATE TABLE NHANVIEN
(
	MaNV VARCHAR(10) PRIMARY KEY,
	HoTen NVARCHAR(50) not null,
	NgSinh DATE not null,
	Phai NVARCHAR(5) not null,
	SDT VARCHAR(11) not null,
	MATKHAU VARCHAR(10) not null
)
GO

CREATE TABLE TOUR
(
	MaTour VARCHAR(10) PRIMARY KEY,
	NgayBD DATE not null,
	NgayKT DATE  not null,
	DiemXuatPhat NVARCHAR(50) not null,
	ChiPhiDauNguoi FLOAT not null,
	SoLuongCon INT null default 20,
	SoLuongBook INT null default 0,
	TinhTrang NVARCHAR(50) null default N'Còn',
	MaHDV VARCHAR(10) not null unique,
	MaDiaDanh VARCHAR(10) not null,
	--MaKhachSan VARCHAR(10) not null,
	--MaPT VARCHAR(10) not null,
	TongTien FLOAT null default 0
)
GO

CREATE TABLE KHACHHANG
(
	MaKH VARCHAR(10) PRIMARY KEY,
	HoTen NVARCHAR(50) not null,
	NgSinh DATE not null,
	Phai NVARCHAR(5) not null,
	SDT VARCHAR(11)  not null,
	Email VARCHAR(50) DEFAULT 'Không có',
	CCCD VARCHAR(12) not null,
	MATKHAU VARCHAR(10) not null
)
GO

CREATE TABLE VE
(
	MaVe VARCHAR(10) PRIMARY KEY,
	NgayBook DATE null default getDATE(),
	ChiPhiDauNguoi FLOAT null,
	MaTour VARCHAR(10) not null,
	MaKH VARCHAR(10) not null,
)
GO

/*==============================================================*/
/*===================== CREATE FOREIGN KEY =====================*/
/*==============================================================*/
ALTER TABLE TOUR
ADD CONSTRAINT FK_MANV_TOUR_NHANVIEN FOREIGN KEY(MaHDV) REFERENCES NHANVIEN(MaNV),
	CONSTRAINT FK_MADIADANH_TOUR_DIEMTHAMQUAN FOREIGN KEY(MaDiaDanh) REFERENCES DIEMTHAMQUAN(MaDiaDanh)
GO

ALTER TABLE VE
ADD CONSTRAINT FK_MaTour_VE_Tour FOREIGN KEY(MaTour) REFERENCES Tour(MaTour),
	CONSTRAINT FK_MaKH_VE_KHACHHANG FOREIGN KEY(MaKH) REFERENCES KHACHHANG(MAKH)
GO

/*==============================================================*/
/*======================== CREATE TRIGGER ======================*/
/*==============================================================*/
--- update so luong con trong tour khi them hoac xoa ve
CREATE TRIGGER UPD_SOLUONGCON ON VE
FOR INSERT, DELETE
AS
	BEGIN
	IF EXISTS(SELECT * FROM inserted)
		BEGIN
			UPDATE TOUR
			SET SOLUONGCON = SOLUONGCON - 1 
			WHERE MaTour = (SELECT inserted.MaTour FROM inserted)
		END
	IF EXISTS(SELECT * FROM deleted)
		BEGIN
			UPDATE TOUR
			SET SOLUONGCON = SOLUONGCON + 1 
			WHERE MaTour = (SELECT deleted.MaTour FROM deleted)
		END
	END
GO

--- update so luong book trong tour khi them hoac xoa ve
CREATE TRIGGER UPD_SOLUONGBOOK ON VE
FOR INSERT, DELETE
AS
	BEGIN
	IF EXISTS(SELECT * FROM inserted)
		BEGIN
			UPDATE TOUR
			SET SoLuongBook = SoLuongBook + 1 
			WHERE MaTour = (SELECT inserted.MaTour FROM inserted)
		END
	IF EXISTS(SELECT * FROM deleted)
		BEGIN
			UPDATE TOUR
			SET SoLuongBook = SoLuongBook - 1 
			WHERE MaTour = (SELECT deleted.MaTour FROM deleted)
		END
	END
GO

--- update tong tien trong tour khi them hoac xoa ve
CREATE TRIGGER UPD_TONGTIEN ON VE
FOR INSERT, DELETE
AS
	BEGIN
	IF EXISTS(SELECT * FROM inserted)
		BEGIN
			UPDATE TOUR SET TongTien = SoLuongBook * ChiPhiDauNguoi
			WHERE MaTour = (SELECT inserted.MaTour FROM inserted)
		END
	IF EXISTS(SELECT * FROM deleted)
		BEGIN
			UPDATE TOUR SET TongTien = SoLuongBook * ChiPhiDauNguoi
			WHERE MaTour = (SELECT inserted.MaTour FROM inserted)
		END
	END
GO

--- update chi phi dau nguoi trong ve
CREATE TRIGGER UPD_CHIPHIDAUNGUOI ON VE
FOR INSERT
AS
	BEGIN
		UPDATE VE 
		SET ChiPhiDauNguoi = (SELECT ChiPhiDauNguoi 
								FROM TOUR 
								WHERE VE.MaTour = TOUR.MaTour)
		WHERE VE.MaTour = (SELECT inserted.MaTour FROM inserted)
	END
GO

/*==============================================================*/
/*========================== INSERT DATA =======================*/
/*==============================================================*/

INSERT INTO DIEMTHAMQUAN(MaDiaDanh, TenDiaDanh, Diachi, HinhAnh, ChiPhi)
VALUES
('DD001',N'Chùa Thiên Mụ',			N'Huế',		N'ChuaThienMu.jpg', 100000),
('DD002',N'Lăng Tự Đức',			N'Huế',		N'LangTuDuc.jpg', 130000),
('DD003',N'Hồ Gươm',				N'Hà Nội',	N'HoGuom.jpg', 150000),
('DD004',N'Lăng Khải Định',			N'Huế',		N'LangKhaiDinh.jpg', 200000),
('DD005',N'Đại Nội Cung Đình Huế',	N'Huế',		N'DaiNoiCungDinhHue.jpg', 220000),
('DD006',N'Văn Miếu – Quốc Tử Giám',N'Hà Nội',	N'VanMieuQuocTuGiam.jpg', 110000),
('DD007',N'Nhà Hát lớn Hà Nội',		N'Hà Nội',	N'NhaHatLonHN.jpg', 140000),
('DD008',N'Hoành Thành Thăng Long',	N'Hà Nội',	N'HoanhThanhThangLong.jpg', 170000),
('DD009',N'Bản Cát Cát',			N'Sapa',	N'BanCatCat.jpg', 50000),
('DD010',N'Cổng trời Sapa',			N'Sapa',	N'CongTroiSapa.jpg', 240000),
('DD011',N'Bản Ý Linh Hồ',			N'Sapa',	N'BanYLinhHo.jpg', 500000),
('DD012',N'Đèo Ô Quy Hồ Sapa',		N'Sapa',	N'DeoOQuyHoSapa.jpg', 100000),
('DD013',N'Rừng Quốc gia Bình Châu',N'Vũng Tàu',N'RungQuocGiaBinhChau.jpg', 100000),
('DD014',N'Hồ đá xanh',				N'Vũng Tàu',N'HoDaXanh.jpg', 140000),
('DD015',N'Khu du lịch Hồ Mây',		N'Vũng Tàu',N'KhuDuLichHoMay.jpg', 700000),
('DD016',N'Nhà úp ngược',			N'Vũng Tàu',N'NhaUpNguoc.jpg', 1000000),
('DD017',N'Hòn Bà',					N'Vũng Tàu',N'HonBa.jpg', 1500000)
SELECT * FROM DIEMTHAMQUAN
GO

SET DATEFORMAT DMY
INSERT INTO KHACHHANG
VALUES
('KH0001', N'Nguyễn Văn Nam',	'12-07-1988',N'Nam','0961078994',	'namnguyen20@gmail.com',		'07200134211','KH0001'),
('KH0002', N'Nguyễn Kim Ánh',	'10-02-1990',N'Nữ',	'02466888333',	'anhbaby@gmail.com',			'07221102341','KH0002'),
('KH0003', N'Nguyễn Thị Châu',	'12-10-1979',N'Nữ',	'0908761086',	'chaulannguyen1210@gmail.com',	'07221702111','KH0003'),
('KH0004', N'Trần Văn Út',		'23-08-1977',N'Nam','0786956666',	'utbe230@gmail.com',			'0723321141','KH0004'),
('KH0005', N'Trần Lệ Quyên',	'22-12-1987',N'Nữ',	'0908761086',	'thongocne33@gmail.com',		'07210202101','KH0005'),
('KH0006', N'Bùi Đức Chí',		'22-12-1987',N'Nam','0798888772',	'chibuicaotho@gmail.com',		'07221001234','KH0006'),
('KH0007', N'Nguyễn Tuấn Anh',	'06-09-1991',N'Nam','0768311999',	'NTAhufi@gmail.com',			'072001102131','KH0007'),
('KH0008', N'Đổ Xuân Thủy',		'14-05-1985',N'Nam','0901294766',	NULL,							'07220104401','KH0008'),
('KH0009', N'Trần Minh Tú',		'17-09-1985',N'Nam','0762302999',	NULL,							'07210752231','KH0009'),
('KH0010', N'Trần Khánh Ngọc',	'13-11-1987',N'Nữ',	'0786666577',	'NgocBFG2@gmail.com',			'07882083012','KH0010')
SELECT * FROM KHACHHANG
GO

SET DATEFORMAT DMY
INSERT INTO NHANVIEN
VALUES
('NV001', N'Trịnh Thăng Bình',	'06-12-1998',N'Nam','0914613234','NV001'),
('NV002', N'Trấn Thành',		'28-01-1995',N'Nam','0853714714','NV002'),
('NV003', N'Trường Giang',		'12-11-1989',N'Nam','0911278833','NV003'),
('NV004', N'Hiếu Thứ Hai',		'05-05-1979',N'Nam','0888111693','NV004'),
('NV005', N'Lee Minho Đồng Nai','13-10-1992',N'Nam','0902220003','NV005'),
('NV006', N'Kiều Minh Tuấn',	'08-06-1990',N'Nam','0853711134','NV006'),
('NV007', N'Cris Devil Gamer',	'22-01-1999',N'Nam','0912258833','NV007'),
('NV008', N'Ngô Kiến Huy',		'28-02-1988',N'Nam','0888444693','NV008'),
('NV009', N'Jack 5 triệu',		'12-12-1970',N'Nam','0902022323','NV009')
SELECT * FROM NHANVIEN
GO

SET DATEFORMAT DMY
INSERT INTO TOUR (MaTour, NgayBD, NgayKT, DiemXuatPhat, ChiPhiDauNguoi, SoLuongCon, MaHDV, MaDiaDanh)
VALUES
('T001','06/12/2022','11/12/2022',N'TPHCM',1000000,20,'NV001','DD001'),
('T002','25/11/2022','28/11/2022',N'TPHCM',2000000,20,'NV002','DD001'),
('T003','22/12/2022','26/12/2022',N'TPHCM',1500000,20,'NV003','DD002'),
('T004','12/01/2023','17/01/2023',N'TPHCM',3000000,20,'NV004','DD003'),
('T005','13/02/2023','19/02/2023',N'TPHCM',5000000,20,'NV005','DD004'),
('T006','13/02/2023','19/02/2023',N'TPHCM',5000000,20,'NV006','DD003'),
('T007','13/02/2023','19/02/2023',N'TPHCM',5000000,20,'NV007','DD015'),
('T008','13/02/2023','19/02/2023',N'TPHCM',5000000,20,'NV008','DD001'),
('T009','13/02/2023','19/02/2023',N'TPHCM',5000000,20,'NV009','DD011')
SELECT * FROM TOUR
GO

INSERT INTO VE(MaVe, NgayBook, MaTour, MaKH)
VALUES
('V001',getdate(),'T001','KH0001')
GO
INSERT INTO VE(MaVe, NgayBook, MaTour, MaKH)
VALUES
('V002',getdate(),'T002','KH0002')
GO
INSERT INTO VE(MaVe, NgayBook, MaTour, MaKH)
VALUES
('V003',getdate(),'T003','KH0002')
GO
INSERT INTO VE(MaVe, NgayBook, MaTour, MaKH)
VALUES
('V004',getdate(),'T003','KH0004')
GO
INSERT INTO VE(MaVe, NgayBook, MaTour, MaKH)
VALUES
('V005',getdate(),'T003','KH0005')
GO
INSERT INTO VE(MaVe, NgayBook, MaTour, MaKH)
VALUES
('V006',getdate(),'T001','KH0002')
GO
INSERT INTO VE(MaVe, NgayBook, MaTour, MaKH)
VALUES
('V007',getdate(),'T002','KH0005')
GO
INSERT INTO VE(MaVe, NgayBook, MaTour, MaKH)
VALUES
('V008',getdate(),'T005','KH0003')
INSERT INTO VE(MaVe, NgayBook, MaTour, MaKH)
VALUES
('V009',getdate(),'T007','KH0007')
INSERT INTO VE(MaVe, NgayBook, MaTour, MaKH)
VALUES
('V010',getdate(),'T006','KH0008')
GO
INSERT INTO VE(MaVe, NgayBook, MaTour, MaKH)
VALUES
('V011',getdate(),'T007','KH0010')
GO
INSERT INTO VE(MaVe, NgayBook, MaTour, MaKH)
VALUES
('V012',getdate(),'T008','KH0001')
GO
SELECT * FROM VE
GO

select * from DIEMTHAMQUAN
select * from TOUR
select * from KHACHHANG
