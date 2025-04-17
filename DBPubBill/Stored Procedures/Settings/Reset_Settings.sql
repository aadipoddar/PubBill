CREATE PROCEDURE [dbo].[Reset_Settings]
AS
BEGIN
	DELETE FROM [Settings]

	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'PubOpenTime', N'17')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'PubCloseTime', N'5')

	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'InactivityTime', N'5')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'RefreshBillTimer', N'30')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'RefreshKOTTimer', N'30')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'RefreshReportTimer', N'60')

	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'PageWidthThermal', N'280')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'PagePaddingTopThermal', N'20')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'PagePaddingBottomThermal', N'20')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'PagePaddingLeftThermal', N'20')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'PagePaddingRightThermal', N'20')

	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'HeaderFontFamilyThermal', N'Arial')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'HeaderFontSizeThermal', N'25')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'HeaderFontWeightThermal', N'Bold')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'HeaderFontPaddingTopThermal', N'0')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'HeaderFontPaddingBottomThermal', N'0')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'HeaderFontPaddingLeftThermal', N'0')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'HeaderFontPaddingRightThermal', N'0')

	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SubHeaderFontFamilyThermal', N'Arial')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SubHeaderFontSizeThermal', N'15')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SubHeaderFontWeightThermal', N'Bold')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SubHeaderFontPaddingTopThermal', N'10')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SubHeaderFontPaddingBottomThermal', N'0')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SubHeaderFontPaddingLeftThermal', N'0')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SubHeaderFontPaddingRightThermal', N'0')

	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'RegularFontFamilyThermal', N'Courier New')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'RegularFontSizeThermal', N'15')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'RegularFontWeightThermal', N'Bold')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'RegularFontPaddingTopThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'RegularFontPaddingBottomThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'RegularFontPaddingLeftThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'RegularFontPaddingRightThermal', N'2')

	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'TableHeaderFontFamilyThermal', N'Arial')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'TableHeaderFontSizeThermal', N'10')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'TableHeaderFontWeightThermal', N'Bold')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'TableHeaderFontPaddingTopThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'TableHeaderFontPaddingBottomThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'TableHeaderFontPaddingLeftThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'TableHeaderFontPaddingRightThermal', N'2')

	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'TableRowFontFamilyThermal', N'Courier New')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'TableRowFontSizeThermal', N'10')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'TableRowFontWeightThermal', N'Bold')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'TableRowFontPaddingTopThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'TableRowFontPaddingBottomThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'TableRowFontPaddingLeftThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'TableRowFontPaddingRightThermal', N'2')

	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'FooterFontFamilyThermal', N'Courier New')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'FooterFontSizeThermal', N'12')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'FooterFontWeightThermal', N'Bold')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'FooterFontPaddingTopThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'FooterFontPaddingBottomThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'FooterFontPaddingLeftThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'FooterFontPaddingRightThermal', N'2')

	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SeparatorFontFamilyThermal', N'Courier New')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SeparatorFontSizeThermal', N'15')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SeparatorFontWeightThermal', N'Bold')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SeparatorFontPaddingTopThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SeparatorFontPaddingBottomThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SeparatorFontPaddingLeftThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SeparatorFontPaddingRightThermal', N'2')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'SeparatorDashCountThermal', N'25')

	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'HeaderLine1', N'GST NO: XXXXXXX')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'HeaderLine2', N'Golden Park Kolkata')
	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'HeaderLine3', N'Mobile No: XXXXXX')

	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'FooterLine', N'Thanks. Vist Again')

	INSERT INTO [dbo].[Settings] ([Key], [Value]) VALUES (N'UPIId', N'upi://pay?pa=aadipoddarmail@okaxis&pn=Aadi%20Poddar')

END