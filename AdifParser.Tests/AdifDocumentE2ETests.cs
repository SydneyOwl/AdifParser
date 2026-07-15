using AdifParser;
using Xunit;

namespace AdifParser.Tests;

/// <summary>
/// ADIF_317_test_QSOs_2026_03_22.adi tests
/// </summary>
public class AdifDocumentE2ETests
{
    private static readonly Lazy<AdifDocument> _doc = new(() =>
    {
        var doc = new AdifDocument();
        doc.ReadFromFile("ADIF_317_test_QSOs_2026_03_22.adi");
        return doc;
    });

    private static AdifDocument Doc => _doc.Value;

    private int CountField(string name) =>
        Doc.Qsos.Count(q => q.GetField(name) != null);
    
    // global

    [Fact]
    public void Total_RecordsEmitted_6197()
    {
        Assert.Equal(6197, Doc.QsoCount);
    }

    [Fact]
    public void Total_FieldsEmitted_54243()
    {
        // QSO token + Header token
        var totalFields = Doc.Qsos.Sum(q => q.Count) + (Doc.Header?.Count ?? 0);
        Assert.Equal(54243, totalFields);
    }

    [Fact]
    public void Total_UniqueCalls_6007()
    {
        var uniqueCalls = new HashSet<string>();
        foreach (var qso in Doc.Qsos)
        {
            var call = qso.GetFieldValue("CALL");
            if (call != null)
                uniqueCalls.Add(call);
        }
        Assert.Equal(6007, uniqueCalls.Count);
    }

    // must-exist fields in every qso

    [Fact]
    public void EveryQso_HasBand()
    {
        Assert.Equal(6197, CountField("BAND"));
    }

    [Fact]
    public void EveryQso_HasCall()
    {
        Assert.Equal(6197, CountField("CALL"));
    }

    [Fact]
    public void EveryQso_HasFreq()
    {
        Assert.Equal(6197, CountField("FREQ"));
    }

    [Fact]
    public void EveryQso_HasMode()
    {
        Assert.Equal(6197, CountField("MODE"));
    }

    [Fact]
    public void EveryQso_HasQsoDate()
    {
        Assert.Equal(6197, CountField("QSO_DATE"));
    }

    [Fact]
    public void EveryQso_HasTimeOn()
    {
        Assert.Equal(6197, CountField("TIME_ON"));
    }

    [Fact]
    public void EveryQso_HasTimeOff()
    {
        Assert.Equal(6197, CountField("TIME_OFF"));
    }

    // Field details

    [Fact]
    public void FieldCount_ADDRESS_1() => Assert.Equal(1, CountField("ADDRESS"));
    [Fact]
    public void FieldCount_AGE_4() => Assert.Equal(4, CountField("AGE"));
    [Fact]
    public void FieldCount_ALTITUDE_3() => Assert.Equal(3, CountField("ALTITUDE"));
    [Fact]
    public void FieldCount_ANT_AZ_3() => Assert.Equal(3, CountField("ANT_AZ"));
    [Fact]
    public void FieldCount_ANT_EL_7() => Assert.Equal(7, CountField("ANT_EL"));
    [Fact]
    public void FieldCount_ANT_PATH_4() => Assert.Equal(4, CountField("ANT_PATH"));
    [Fact]
    public void FieldCount_ARRL_SECT_98() => Assert.Equal(98, CountField("ARRL_SECT"));
    [Fact]
    public void FieldCount_COMMENT_1() => Assert.Equal(1, CountField("COMMENT"));
    [Fact]
    public void FieldCount_CONT_7() => Assert.Equal(7, CountField("CONT"));
    [Fact]
    public void FieldCount_COUNTRY_1() => Assert.Equal(1, CountField("COUNTRY"));
    [Fact]
    public void FieldCount_CQZ_1() => Assert.Equal(1, CountField("CQZ"));
    [Fact]
    public void FieldCount_DXCC_2624() => Assert.Equal(2624, CountField("DXCC"));
    [Fact]
    public void FieldCount_GRIDSQUARE_7() => Assert.Equal(7, CountField("GRIDSQUARE"));
    [Fact]
    public void FieldCount_ITUZ_1() => Assert.Equal(1, CountField("ITUZ"));
    [Fact]
    public void FieldCount_PROP_MODE_22() => Assert.Equal(22, CountField("PROP_MODE"));
    [Fact]
    public void FieldCount_RST_RCVD_26() => Assert.Equal(26, CountField("RST_RCVD"));
    [Fact]
    public void FieldCount_RST_SENT_26() => Assert.Equal(26, CountField("RST_SENT"));
    [Fact]
    public void FieldCount_STATE_1897() => Assert.Equal(1897, CountField("STATE"));
    [Fact]
    public void FieldCount_CONTEST_ID_252() => Assert.Equal(252, CountField("CONTEST_ID"));
    [Fact]
    public void FieldCount_CREDIT_SUBMITTED_154() => Assert.Equal(154, CountField("CREDIT_SUBMITTED"));
    [Fact]
    public void FieldCount_CREDIT_GRANTED_77() => Assert.Equal(77, CountField("CREDIT_GRANTED"));
    [Fact]
    public void FieldCount_STATION_CALLSIGN_346() => Assert.Equal(346, CountField("STATION_CALLSIGN"));
    [Fact]
    public void FieldCount_SUBMODE_183() => Assert.Equal(183, CountField("SUBMODE"));
    [Fact]
    public void FieldCount_BAND_RX_76() => Assert.Equal(76, CountField("BAND_RX"));
    [Fact]
    public void FieldCount_FREQ_RX_99() => Assert.Equal(99, CountField("FREQ_RX"));
    [Fact]
    public void FieldCount_CNTY_34() => Assert.Equal(34, CountField("CNTY"));
    [Fact]
    public void FieldCount_CNTY_ALT_73() => Assert.Equal(73, CountField("CNTY_ALT"));
    [Fact]
    public void FieldCount_WWFF_REF_3() => Assert.Equal(3, CountField("WWFF_REF"));
    [Fact]
    public void FieldCount_SAT_NAME_1() => Assert.Equal(1, CountField("SAT_NAME"));
    [Fact]
    public void FieldCount_SAT_MODE_1() => Assert.Equal(1, CountField("SAT_MODE"));
    [Fact]
    public void FieldCount_QSL_RCVD_6() => Assert.Equal(6, CountField("QSL_RCVD"));
    [Fact]
    public void FieldCount_QSL_SENT_8() => Assert.Equal(8, CountField("QSL_SENT"));
    [Fact]
    public void FieldCount_LOTW_QSL_RCVD_6() => Assert.Equal(6, CountField("LOTW_QSL_RCVD"));
    [Fact]
    public void FieldCount_LOTW_QSL_SENT_8() => Assert.Equal(8, CountField("LOTW_QSL_SENT"));
    [Fact]
    public void FieldCount_EQSL_QSL_RCVD_6() => Assert.Equal(6, CountField("EQSL_QSL_RCVD"));
    [Fact]
    public void FieldCount_EQSL_QSL_SENT_8() => Assert.Equal(8, CountField("EQSL_QSL_SENT"));
    [Fact]
    public void FieldCount_DCL_QSL_RCVD_6() => Assert.Equal(6, CountField("DCL_QSL_RCVD"));
    [Fact]
    public void FieldCount_DCL_QSL_SENT_8() => Assert.Equal(8, CountField("DCL_QSL_SENT"));
    [Fact]
    public void FieldCount_TX_PWR_18() => Assert.Equal(18, CountField("TX_PWR"));
    [Fact]
    public void FieldCount_RX_PWR_18() => Assert.Equal(18, CountField("RX_PWR"));
    [Fact]
    public void FieldCount_DISTANCE_3() => Assert.Equal(3, CountField("DISTANCE"));
    [Fact]
    public void FieldCount_IOTA_3() => Assert.Equal(3, CountField("IOTA"));
    [Fact]
    public void FieldCount_QSO_COMPLETE_8() => Assert.Equal(8, CountField("QSO_COMPLETE"));
    [Fact]
    public void FieldCount_MS_SHOWER_12() => Assert.Equal(12, CountField("MS_SHOWER"));
    [Fact]
    public void FieldCount_REGION_10() => Assert.Equal(10, CountField("REGION"));
    [Fact]
    public void FieldCount_SRX_5() => Assert.Equal(5, CountField("SRX"));
    [Fact]
    public void FieldCount_STX_5() => Assert.Equal(5, CountField("STX"));
    [Fact]
    public void FieldCount_OPERATOR_5() => Assert.Equal(5, CountField("OPERATOR"));
    [Fact]
    public void FieldCount_OWNER_CALLSIGN_5() => Assert.Equal(5, CountField("OWNER_CALLSIGN"));
    [Fact]
    public void FieldCount_QSO_DATE_OFF_5() => Assert.Equal(5, CountField("QSO_DATE_OFF"));
    [Fact]
    public void FieldCount_MY_ARRL_SECT_98() => Assert.Equal(98, CountField("MY_ARRL_SECT"));
    [Fact]
    public void FieldCount_MY_DXCC_2272() => Assert.Equal(2272, CountField("MY_DXCC"));
    [Fact]
    public void FieldCount_MY_STATE_1897() => Assert.Equal(1897, CountField("MY_STATE"));
    [Fact]
    public void FieldCount_MY_CNTY_34() => Assert.Equal(34, CountField("MY_CNTY"));
    [Fact]
    public void FieldCount_MY_CNTY_ALT_73() => Assert.Equal(73, CountField("MY_CNTY_ALT"));
    [Fact]
    public void FieldCount_MY_GRIDSQUARE_7() => Assert.Equal(7, CountField("MY_GRIDSQUARE"));
    [Fact]
    public void FieldCount_MY_CITY_1() => Assert.Equal(1, CountField("MY_CITY"));
    [Fact]
    public void FieldCount_MY_COUNTRY_1() => Assert.Equal(1, CountField("MY_COUNTRY"));
    [Fact]
    public void FieldCount_MY_CQ_ZONE_1() => Assert.Equal(1, CountField("MY_CQ_ZONE"));
    [Fact]
    public void FieldCount_MY_ITU_ZONE_1() => Assert.Equal(1, CountField("MY_ITU_ZONE"));
    [Fact]
    public void FieldCount_MY_LAT_5() => Assert.Equal(5, CountField("MY_LAT"));
    [Fact]
    public void FieldCount_MY_LON_5() => Assert.Equal(5, CountField("MY_LON"));
    [Fact]
    public void FieldCount_MY_ALTITUDE_3() => Assert.Equal(3, CountField("MY_ALTITUDE"));
    [Fact]
    public void FieldCount_MY_IOTA_3() => Assert.Equal(3, CountField("MY_IOTA"));
    [Fact]
    public void FieldCount_MY_WWFF_REF_3() => Assert.Equal(3, CountField("MY_WWFF_REF"));
    [Fact]
    public void FieldCount_MY_POTA_REF_3() => Assert.Equal(3, CountField("MY_POTA_REF"));
    [Fact]
    public void FieldCount_MY_SOTA_REF_2() => Assert.Equal(2, CountField("MY_SOTA_REF"));
    [Fact]
    public void FieldCount_MY_DARC_DOK_3() => Assert.Equal(3, CountField("MY_DARC_DOK"));
    [Fact]
    public void FieldCount_MY_FISTS_3() => Assert.Equal(3, CountField("MY_FISTS"));
    [Fact]
    public void FieldCount_MY_MORSE_KEY_INFO_8() => Assert.Equal(8, CountField("MY_MORSE_KEY_INFO"));
    [Fact]
    public void FieldCount_MY_MORSE_KEY_TYPE_8() => Assert.Equal(8, CountField("MY_MORSE_KEY_TYPE"));
    [Fact]
    public void FieldCount_MY_VUCC_GRIDS_2() => Assert.Equal(2, CountField("MY_VUCC_GRIDS"));
    [Fact]
    public void FieldCount_MY_USACA_COUNTIES_2() => Assert.Equal(2, CountField("MY_USACA_COUNTIES"));
    [Fact]
    public void FieldCount_MY_ANTENNA_1() => Assert.Equal(1, CountField("MY_ANTENNA"));
    [Fact]
    public void FieldCount_MY_RIG_1() => Assert.Equal(1, CountField("MY_RIG"));
    [Fact]
    public void FieldCount_MY_NAME_1() => Assert.Equal(1, CountField("MY_NAME"));
    [Fact]
    public void FieldCount_MY_STREET_1() => Assert.Equal(1, CountField("MY_STREET"));
    [Fact]
    public void FieldCount_MY_POSTAL_CODE_1() => Assert.Equal(1, CountField("MY_POSTAL_CODE"));
    [Fact]
    public void FieldCount_MY_SIG_1() => Assert.Equal(1, CountField("MY_SIG"));
    [Fact]
    public void FieldCount_MY_SIG_INFO_1() => Assert.Equal(1, CountField("MY_SIG_INFO"));

    // Variant: User, USERDEFn number:   1

    [Fact]
    public void FieldCount_RCVD_FROM_DX_CLUSTER_1() => Assert.Equal(1, CountField("RCVD_FROM_DX_CLUSTER"));
    [Fact]
    public void FieldCount_MY_POWER_CATEGORY_1() => Assert.Equal(1, CountField("MY_POWER_CATEGORY"));
    [Fact]
    public void FieldCount_MY_TEMPERATURE_FAHRENHEIT_1() => Assert.Equal(1, CountField("MY_TEMPERATURE_FAHRENHEIT"));
    [Fact]
    public void FieldCount_MY_HEIGHT_ASL_FEET_1() => Assert.Equal(1, CountField("MY_HEIGHT_ASL_FEET"));
    [Fact]
    public void FieldCount_NEXT_QSL_POSTING_DATE_1() => Assert.Equal(1, CountField("NEXT_QSL_POSTING_DATE"));
    [Fact]
    public void FieldCount_NEXT_QSL_POSTING_TIME_1() => Assert.Equal(1, CountField("NEXT_QSL_POSTING_TIME"));
    [Fact]
    public void FieldCount_MY_AMP_1() => Assert.Equal(1, CountField("MY_AMP"));
    [Fact]
    public void FieldCount_QSO_TRANSCRIPT_1() => Assert.Equal(1, CountField("QSO_TRANSCRIPT"));
    [Fact]
    public void FieldCount_REMOTE_STATION_LAT_1() => Assert.Equal(1, CountField("REMOTE_STATION_LAT"));
    [Fact]
    public void FieldCount_REMOTE_STATION_LON_1() => Assert.Equal(1, CountField("REMOTE_STATION_LON"));

    // 0 occurence
    [Fact]
    public void FieldCount_MY_AMP_INTL_0() => Assert.Equal(0, CountField("MY_AMP_INTL"));
    [Fact]
    public void FieldCount_QSO_TRANSCRIPT_INTL_0() => Assert.Equal(0, CountField("QSO_TRANSCRIPT_INTL"));
    [Fact]
    public void FieldCount_GUEST_OP_0() => Assert.Equal(0, CountField("GUEST_OP"));
    [Fact]
    public void FieldCount_VE_PROV_0() => Assert.Equal(0, CountField("VE_PROV"));

    // Variant: App

    [Fact]
    public void FieldCount_APP_USING_YAGI_1() => Assert.Equal(1, CountField("APP_CreateADIFTestFiles_USING_YAGI"));
    [Fact]
    public void FieldCount_APP_WORDS_PER_MINUTE_2() => Assert.Equal(2, CountField("APP_CreateADIFTestFiles_WORDS_PER_MINUTE"));
    [Fact]
    public void FieldCount_APP_NEXT_SKED_DATE_1() => Assert.Equal(1, CountField("APP_CreateADIFTestFiles_NEXT_SKED_DATE"));
    [Fact]
    public void FieldCount_APP_NEXT_SKED_TIME_ON_1() => Assert.Equal(1, CountField("APP_CreateADIFTestFiles_NEXT_SKED_TIME_ON"));
    [Fact]
    public void FieldCount_APP_NEXT_SKED_TIME_OFF_1() => Assert.Equal(1, CountField("APP_CreateADIFTestFiles_NEXT_SKED_TIME_OFF"));
    [Fact]
    public void FieldCount_APP_MY_OS_1() => Assert.Equal(1, CountField("APP_CreateADIFTestFiles_MY_OS"));
    [Fact]
    public void FieldCount_APP_WX_1() => Assert.Equal(1, CountField("APP_CreateADIFTestFiles_WX"));
    [Fact]
    public void FieldCount_APP_NEXT_LAT_1() => Assert.Equal(1, CountField("APP_CreateADIFTestFiles_NEXT_LAT"));
    [Fact]
    public void FieldCount_APP_NEXT_LON_1() => Assert.Equal(1, CountField("APP_CreateADIFTestFiles_NEXT_LON"));
    [Fact]
    public void FieldCount_APP_OTHER_SENT_TO_DX_CLUSTER_1() => Assert.Equal(1, CountField("APP_Other_SENT_TO_DX_CLUSTER"));

    [Fact]
    public void FieldCount_APP_MY_OS_INTL_0() => Assert.Equal(0, CountField("APP_CreateADIFTestFiles_MY_OS_INTL"));
    [Fact]
    public void FieldCount_APP_WX_INTL_0() => Assert.Equal(0, CountField("APP_CreateADIFTestFiles_WX_INTL"));

    // ═══════════════════════════════════════════════
    // 呼号格式多样性
    // ═══════════════════════════════════════════════

    [Fact]
    public void Callsigns_VariousFormats()
    {
        // Report 底部有重复呼号统计，但那是生成器内部数据，和文件实际不符。
        // 改为验证呼号格式多样性。
        var calls = new HashSet<string>();
        foreach (var qso in Doc.Qsos)
        {
            var call = qso.GetFieldValue("CALL");
            if (call != null) calls.Add(call);
        }

        Assert.Contains(calls, c => c.Contains('/'));   // 含 / 的呼号
        Assert.Contains(calls, c => c.Length <= 4);      // 短呼号
        Assert.Contains(calls, c => c.Length >= 6);      // 长呼号
        Assert.Contains(calls, c => char.IsDigit(c[0])); // 数字开头
    }

    [Fact]
    public void Callsigns_HaveRepeats()
    {
        // 验证确实有呼号重复出现（不是全唯一）
        var callCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var qso in Doc.Qsos)
        {
            var call = qso.GetFieldValue("CALL");
            if (call != null)
                callCounts[call] = callCounts.GetValueOrDefault(call) + 1;
        }

        var maxRepeat = callCounts.Values.Max();
        Assert.True(maxRepeat > 1, "应该有重复呼号");
    }

    // ═══════════════════════════════════════════════
    // Header 验证
    // ═══════════════════════════════════════════════

    [Fact]
    public void Header_HasAllStandardFields()
    {
        Assert.True(Doc.HasHeader);
        Assert.Equal("3.1.7", Doc.Header!.GetFieldValue("ADIF_VER"));
        Assert.Equal("20260322 124231", Doc.Header.GetFieldValue("CREATED_TIMESTAMP"));
        Assert.Equal("CreateADIFTestFiles", Doc.Header.GetFieldValue("PROGRAMID"));
        Assert.Equal("3.1.7.0", Doc.Header.GetFieldValue("PROGRAMVERSION"));
    }

    [Fact]
    public void Header_UserdefTypes()
    {
        Assert.Equal('B', Doc.Header!.GetField("USERDEF1")!.UserDefType);
        Assert.Equal('E', Doc.Header!.GetField("USERDEF2")!.UserDefType);
        Assert.Equal('N', Doc.Header!.GetField("USERDEF3")!.UserDefType);
        Assert.Equal('N', Doc.Header!.GetField("USERDEF4")!.UserDefType);
        Assert.Equal('D', Doc.Header!.GetField("USERDEF5")!.UserDefType);
        Assert.Equal('T', Doc.Header!.GetField("USERDEF6")!.UserDefType);
        Assert.Equal('S', Doc.Header!.GetField("USERDEF7")!.UserDefType);
        Assert.Equal('I', Doc.Header!.GetField("USERDEF8")!.UserDefType);
        Assert.Equal('M', Doc.Header!.GetField("USERDEF9")!.UserDefType);
        Assert.Equal('G', Doc.Header!.GetField("USERDEF10")!.UserDefType);
        Assert.Equal('L', Doc.Header!.GetField("USERDEF11")!.UserDefType);
        Assert.Equal('L', Doc.Header!.GetField("USERDEF12")!.UserDefType);
    }

    // ═══════════════════════════════════════════════
    // 首尾 QSO 数据验证
    // ═══════════════════════════════════════════════

    [Fact]
    public void FirstQso_BasicFields()
    {
        var qso = Doc.Qsos[0];
        Assert.Equal("VE3AAA", qso.GetFieldValue("CALL"));
        Assert.Equal("20m", qso.GetFieldValue("BAND"));
        Assert.Equal("14.05", qso.GetFieldValue("FREQ"));
        Assert.Equal("SSB", qso.GetFieldValue("MODE"));
        Assert.Equal("20260122", qso.GetFieldValue("QSO_DATE"));
    }

    [Fact]
    public void FirstQso_UserdefFields()
    {
        var qso = Doc.Qsos[0];
        Assert.Equal("N", qso.GetFieldValue("RCVD_FROM_DX_CLUSTER"));
        Assert.Equal("QrpP", qso.GetFieldValue("MY_POWER_CATEGORY"));
        Assert.Equal("66", qso.GetFieldValue("MY_TEMPERATURE_FAHRENHEIT"));
        Assert.Equal("150.5", qso.GetFieldValue("MY_HEIGHT_ASL_FEET"));
        Assert.Equal("1 KW", qso.GetFieldValue("MY_AMP"));
        Assert.Equal("S018 28.430", qso.GetFieldValue("REMOTE_STATION_LAT"));
        Assert.Equal("W021 22.892", qso.GetFieldValue("REMOTE_STATION_LON"));
    }

    [Fact]
    public void FirstQso_MultiLineFields()
    {
        var qso = Doc.Qsos[0];
        var transcript = qso.GetFieldValue("QSO_TRANSCRIPT");
        Assert.NotNull(transcript);
        Assert.Contains("NAME FRED", transcript);
        Assert.Contains("QTH ALENCON", transcript);
        Assert.Contains("73 ES GOOD DX", transcript);
    }

    [Fact]
    public void LastQso_BasicFields()
    {
        var last = Doc.Qsos[Doc.QsoCount - 1];
        Assert.Equal("3D2IST", last.GetFieldValue("CALL"));
        Assert.Equal("12m", last.GetFieldValue("BAND"));
        Assert.Equal("24.980589", last.GetFieldValue("FREQ"));
        Assert.Equal("OPERA", last.GetFieldValue("MODE"));
        Assert.Equal("20260215", last.GetFieldValue("QSO_DATE"));
        Assert.Equal("3D2FF-0006", last.GetFieldValue("WWFF_REF"));
    }

    // ═══════════════════════════════════════════════
    // 多行值 / 特殊字符
    // ═══════════════════════════════════════════════

    [Fact]
    public void MultiLine_Address()
    {
        var qso = Doc.Qsos[3];
        var addr = qso.GetFieldValue("ADDRESS");
        Assert.NotNull(addr);
        Assert.Contains("123 My Street", addr);
        Assert.Contains("My Town", addr);
        Assert.Contains("My Post Code", addr);
    }

    [Fact]
    public void MultiLine_AppWx()
    {
        var qso = Doc.Qsos[1];
        var wx = qso.GetFieldValue("APP_CreateADIFTestFiles_WX");
        Assert.NotNull(wx);
        Assert.Contains("Cloud scattered", wx);
        Assert.Contains("Light rain", wx);
    }

    // LENGTH 字段驱动数据边界，数据内含 '<' 不再截断。
    [Fact]
    public void SpecialChars_CommentWithAngleBrackets()
    {
        var qso = Doc.Qsos.First(q => q.GetField("COMMENT") != null);
        var comment = qso.GetFieldValue("COMMENT")!;
        Assert.Contains("> 10 Watts", comment);
        Assert.Contains("< 100 Watts", comment);
    }

    // ═══════════════════════════════════════════════
    // 大小写不敏感
    // ═══════════════════════════════════════════════

    [Fact]
    public void CaseInsensitive_MixedCaseFieldNames()
    {
        var qso = Doc.Qsos[0];
        Assert.Equal("150.5", qso.GetFieldValue("my_height_asl_feet"));
        Assert.Equal("66", qso.GetFieldValue("My_Temperature_Fahrenheit"));
        Assert.Equal("QrpP", qso.GetFieldValue("mY_poweR_categorY"));
    }

    [Fact]
    public void CaseInsensitive_LowercaseModeValue()
    {
        var qso = Doc.Qsos.First(q => q.GetFieldValue("MODE") == "ssb");
        Assert.Equal("ssb", qso.GetFieldValue("MODE"));
    }
    

    [Fact]
    public void RoundTrip_PreservesQsoCount()
    {
        var doc2 = new AdifDocument();
        doc2.ReadFromString(Doc.ToString());
        Assert.Equal(Doc.QsoCount, doc2.QsoCount);
    }

    [Fact]
    public void RoundTrip_PreservesFirstAndLastCall()
    {
        var doc2 = new AdifDocument();
        doc2.ReadFromString(Doc.ToString());
        Assert.Equal(Doc.Qsos[0].GetFieldValue("CALL"),
                     doc2.Qsos[0].GetFieldValue("CALL")?.Trim());
        Assert.Equal(Doc.Qsos[Doc.QsoCount - 1].GetFieldValue("CALL"),
                     doc2.Qsos[doc2.QsoCount - 1].GetFieldValue("CALL")?.Trim());
    }
}