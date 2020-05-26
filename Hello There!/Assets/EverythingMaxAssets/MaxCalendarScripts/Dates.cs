﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Globalization;

using System.Collections.Generic;

public partial class Dates : MonoBehaviour
{
    private DayToggle[] DayToggles = new DayToggle[7 * 6];
    private bool dayTogglesGenerated = false;
    public DayToggle DayToggleTemplate;
    public Text DaysOfTheWeek;

    [SerializeField]
    private GridLayoutGroup DayContainer;
    [SerializeField]
    private Text SelectedDateText;
    [SerializeField]
    private Text CurrentMonth;
    [SerializeField]
    private Text CurrentYear;
    public string DateFormat = "MM/dd/yyyy";
    public string MonthFormat = "MMMMM";
    public bool FutureToggleDate = false;

    private DateTime? _SelectedDate;

    public DateTime? SelectedDate
    {
        get { return _SelectedDate; }
        private set
        {
            _SelectedDate = value;
            if (_SelectedDate != null)
            {
                SelectedDateText.text = ((DateTime)_SelectedDate).ToString(DateFormat);
            }
            else
            {
                SelectedDateText.text = string.Empty;
            }
        }
    }
    // DateTime _ReferenceDate = DateTime.Now.AddYears(-100);
    DateTime _ReferenceDate = DateTime.Now.AddYears(0);
    DateTime _DisplayDate = DateTime.Now.AddYears(-1);
    //  DateTime _DisplayDate = DateTime.Now.AddYears(-101);
    public DateTime ReferenceDateTime
    {
        get { return _ReferenceDate; }

        set
        {
            _ReferenceDate = Utilities.GetYearMonthStart(value);
            CurrentYear.text = _ReferenceDate.Year.ToString();
            CurrentMonth.text = _ReferenceDate.ToString(MonthFormat);
        }
    }

    public DayOfWeek startDayOfWeek;
    void Start()
    {
        Debug.Log("this is a test1");
        DaysOfTheWeekNamesController();
        GenerateDaysToggles();
        // Just in case SetSelectedDate is called before the Start function is executed
        if (SelectedDate == null)
        {
            SetSelectedDate(DateTime.Today);
        }
        else { SwitchToSelectedDate(); }
    }

    public string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }


    public void DaysOfTheWeekNamesController()
    {
        int dayOfWeek = (int)startDayOfWeek;
        for (int d = 1; d <= 7; d++)
        {
            string day_name = Truncate(Enum.GetName(typeof(DayOfWeek), dayOfWeek), 3);
            Debug.Log("this is a test2");
            var DayNameLabel = Instantiate(DaysOfTheWeek);
            Debug.Log("this is a test3");
            DayNameLabel.name = String.Format("Day Name Label ({0})", day_name);
            DayNameLabel.transform.SetParent(DayContainer.transform);
            DayNameLabel.GetComponentInChildren<Text>().text = day_name;
            dayOfWeek++;
            if (dayOfWeek >= 7)
            {
                dayOfWeek = 0;
            }
        }
    }
    public void GenerateDaysToggles()
    {
        for (int i = 0; i < DayToggles.Length; i++)
        {
            var DayToggle = Instantiate(DayToggleTemplate);
            DayToggle.transform.SetParent(DayContainer.transform);
            DayToggle.GetComponentInChildren<Text>().text = string.Empty;
            DayToggle.onDateSelected.AddListener(OnDaySelected);
            DayToggles[i] = DayToggle;
        }
        dayTogglesGenerated = true;
    }
    private void DisplayMonthDays(bool refresh = false)
    {
        if (!refresh && _DisplayDate.IsSameYearMonth(ReferenceDateTime))
        {
            return;
        }
        _DisplayDate = ReferenceDateTime.DuplicateDate(ReferenceDateTime);

        int monthdays = ReferenceDateTime.DaysInMonth();

        DateTime day_datetime = _DisplayDate.GetYearMonthStart();

        int dayOffset = (int)day_datetime.DayOfWeek - (int)startDayOfWeek;
        if ((int)day_datetime.DayOfWeek < (int)startDayOfWeek)
        {
            dayOffset = (7 + dayOffset);
        }
        day_datetime = day_datetime.AddDays(-dayOffset);
        //DayContainer.GetComponent<ToggleGroup>().allowSwitchOff = true;
        for (int i = 0; i < DayToggles.Length; i++)
        {
            SetDayToggle(DayToggles[i], day_datetime);
            day_datetime = day_datetime.AddDays(1);
        }
        //DayContainer.GetComponent<ToggleGroup>().allowSwitchOff = false;
    }

    void SetDayToggle(DayToggle dayToggle, DateTime toggleDate)
    {
        dayToggle.interactable = ((!FutureToggleDate || (FutureToggleDate && !toggleDate.IsPast())) && toggleDate.IsSameYearMonth(_DisplayDate));
        dayToggle.name = String.Format("Day Toggle ({0} {1})", toggleDate.ToString("MMM"), toggleDate.Day);
        dayToggle.SetText(toggleDate.Day.ToString());
        dayToggle.dateTime = toggleDate;

        dayToggle.isOn = (SelectedDate != null) && ((DateTime)SelectedDate).IsSameDate(toggleDate);
    }

    public void YearInc_onClick()
    {
        ReferenceDateTime = ReferenceDateTime.AddYears(1);
        DisplayMonthDays(false);
    }
    public void YearDec_onClick()
    {
        if (!FutureToggleDate || (!ReferenceDateTime.IsCurrentYear() && !ReferenceDateTime.IsPastYearMonth()))
        {
            ReferenceDateTime = ReferenceDateTime.AddYears(-1);
            DisplayMonthDays(false);
        }
    }
    public void MonthInc_onClick()
    {
        ReferenceDateTime = ReferenceDateTime.AddMonths(1);
        DisplayMonthDays(false);
    }
    public void MonthDec_onClick()
    {
        if (!FutureToggleDate || (!ReferenceDateTime.IsCurrentYearMonth() && !ReferenceDateTime.IsPastYearMonth()))
        {
            ReferenceDateTime = ReferenceDateTime.AddMonths(-1);
            DisplayMonthDays(false);
        }
    }

    public void SetSelectedDate(DateTime date)
    {
        SelectedDate = date;
        SwitchToSelectedDate();
    }

    public void repeatDate()
    {

    }

    void OnDaySelected(Nullable<DateTime> date)
    {
        SetSelectedDate((DateTime)date);
    }

    public void SwitchToSelectedDate()
    {
        if (SelectedDate != null)
        {
            var _selectedDate = (DateTime)SelectedDate;
            if (!_selectedDate.IsSameYearMonth(_DisplayDate))
            {
                ReferenceDateTime = (DateTime)SelectedDate;
                if (dayTogglesGenerated)
                {
                    DisplayMonthDays(false);
                }
            }
        }
    }
    public void Today_onClick()
    {
        ReferenceDateTime = DateTime.Today;
        DisplayMonthDays(false);
    }
}

