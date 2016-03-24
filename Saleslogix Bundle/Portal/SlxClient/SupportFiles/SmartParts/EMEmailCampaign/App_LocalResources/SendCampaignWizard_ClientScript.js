<script language="javascript" type="text/javascript">
dojo.require("dijit.form.HorizontalSlider");
dojo.require("dijit.form.TextBox");
dojo.require("dojox.charting.Chart2D");
dojo.require("dojox.charting.plot2d.Pie");
dojo.require("dojox.charting.action2d.Highlight");
dojo.require("dojox.charting.action2d.MoveSlice");
dojo.require("dojox.charting.action2d.Tooltip");
dojo.require("dojox.charting.themes.Julie");
dojo.require("dojox.charting.widget.Legend");

// dojo.query does not exist in dojo v1.5 (slx754), so it might not 'require'
try {
	dojo.require("dojo.query");
} catch (ex) {
	// swallow the exception, later code should check if dojo.query is loaded before using it.
}

function pageLoad() {
    
    $('.pageContainer').each(function () {
        var strId = $(this).attr('id');
        if (strId.indexOf('page1') != -1) //We're on page1
        {
            HideBack();
            HideSend();
            DisableNext();
            
            $('input[name$="chkSelect"]').each(function () {
                $(this).click(function () {
                    if ($('input[name$="chkSelect"]:checked').length > 0)
                    {
                        EnableNext();
                    }
                    else
                    {
                        DisableNext();
                    }
                });
            });         
        }
        else if (strId.indexOf('page2') != -1) //We're on page2
        {
            EnableNext();
            ShowNext();
            ShowBack();
            HideSend();

            var linebreak = '';
            if (navigator.appName == 'Netscape')
            {
                linebreak = '<br>';
            }
            else
            {
                linebreak = '\n';
            }

            var duplicateTargets = $('span[id$="SendEmailCampaignWizard_LessDuplicateTargets"]').html();
            var doNotSolicitTargets = $('span[id$="SendEmailCampaignWizard_LessTargetsDoNotSolicit"]').html();
            var noEmailTargets = $('span[id$="SendEmailCampaignWizard_LessTargetsNoEmail"]').html();
            var suppressedTargets = parseInt(duplicateTargets) + parseInt(doNotSolicitTargets) + parseInt(noEmailTargets);

			if (suppressedTargets > 0) {
				var duplicatePercentage = Math.round((duplicateTargets / suppressedTargets) * 100);
				var doNotSolicitPercentage = Math.round((doNotSolicitTargets / suppressedTargets) * 100);
				var noEmailPercentage = Math.round((noEmailTargets / suppressedTargets) * 100);
	
				var dc = dojox.charting;
				var suppressionChart = new dc.Chart2D("suppressionChart");
				suppressionChart.setTheme(dc.themes.Julie)
					.addPlot("default", {
					type: "Pie",
					font: "normal normal 8pt Tahoma",
					fontColor: "black",
					labelOffset: -35,
					radius: 45
				}).addSeries("Series A", [
					{y: duplicateTargets, text: "Duplicates:" + linebreak + duplicateTargets + " (" + duplicatePercentage + "%)",   stroke: "black"},
					{y: doNotSolicitTargets, text: "Do Not Solicit/Email:" + linebreak + doNotSolicitTargets + " (" + doNotSolicitPercentage + "%)", stroke: "black"},
					{y: noEmailTargets, text: "No Email:" + linebreak + noEmailTargets + " (" + noEmailPercentage + "%)",  stroke: "black"}
				]);
				//var anim_a = new dc.action2d.MoveSlice(suppressionChart, "default");
				var anim_b = new dc.action2d.Highlight(suppressionChart, "default");
				//var anim_c = new dc.action2d.Tooltip(suppressionChart, "default");
				suppressionChart.render();
			}
			
			// If there are no Total Unique Targets, do not allow user to proceed
			var uniqueTargets = $('span[id$="SendEmailCampaignWizard_TotalUniqueTargets"]').html();
			if (parseInt(uniqueTargets) > 0)
            {
                EnableNext();
            }
            else
            {
                DisableNext();
            }
        }
        else if (strId.indexOf('page3') != -1) //We're on page3
        {
            EnableNext();
            ShowNext();
            ShowBack();
            HideSend();
            DisablePercentage();
            
            if ($('input[value="Immediately"]').attr('checked'))
            {
                DisableSendTime();
                FormatSummarySendTime('Immediately', null);
            }
            else
            {
                EnableSendTime();
                FormatSummarySendTime('Scheduled', $('input[id$="SendEmailCampaignWizard_dntSendTime_TXT"]').val());
            }

            $('input[type="radio"]').each(function () {
                $(this).click(function () {
                    if ($(this).attr('value') == 'Scheduled')
                    {
                        EnableSendTime();
                        FormatSummarySendTime('Scheduled', $('input[id$="SendEmailCampaignWizard_dntSendTime_TXT"]').val());
                    }
                    else if ($(this).attr('value') == 'Immediately')
                    {
                        DisableSendTime();
                        FormatSummarySendTime('Immediately', null);
                    }
                });
            });

            $('input[id$="SendEmailCampaignWizard_dntSendTime_TXT"]').change(function () {
                FormatSummarySendTime('Scheduled', $(this).val());
            });

            FormatSummarySplit();
            /*
            $('input[id$="SendEmailCampaignWizard_txtWaitHours"]').change(function () {
                FormatSummarySplit();
            });
            */
            $('input[id$="SendEmailCampaignWizard_txtWaitHours"]').keydown(function(event) {
                // Allow: backspace, delete, tab, escape, and enter
                if ( event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 || 
                     // Allow: Ctrl+A
                    (event.keyCode == 65 && event.ctrlKey === true) || 
                     // Allow: home, end, left, right
                    (event.keyCode >= 35 && event.keyCode <= 39)) {
                         // let it happen, don't do anything
                         return;
                }
                else {
                    // Ensure that it is a number and stop the keypress
                    if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105 )) {
                        event.preventDefault(); 
                    }   
                }
            });

            if ($('input[id$="SendEmailCampaignWizard_txtWaitHours"]').val() < 1)
            {
                DisableNext();
            }
            else
            {
                EnableNext();
            }

            $('input[id$="SendEmailCampaignWizard_txtWaitHours"]').keyup(function(event) {
                if ($(this).val() > 24)
                {
                    $(this).val(24);
                }
                if ($(this).val() < 1)
                {
                    DisableNext();
                }
                else
                {
                    EnableNext();
                }
                FormatSummarySplit();
            });

            if ($('#slider').length > 0)
            {
                var sliderStartingValue = $('input[id$="SendEmailCampaignWizard_txtTargetPercentage"]').val();
                if (sliderStartingValue < 1)
                {
                    sliderStartingValue = 1;
                }
            
                var slider = new dijit.form.HorizontalSlider({
                    name:"slider",
                    value:sliderStartingValue,
                    minimum:1,
                    maximum:60,
                    discreteValues:60,
                        intermediateChanges: true,
                    style: "width:300px;",
                    onChange: function(value){
						if (dojo.query) {
                            // SLX 8
                            dojo.query("[id$=SendEmailCampaignWizard_txtTargetPercentage]")[0].value = value;
                        } else {
                            // SLX 7.5
                            dojo.byId("ctl00_DialogWorkspace_SendEmailCampaignWizard_txtTargetPercentage").value = value;
                        }
						
                        FormatSummarySplit();
                    }
                }, "slider");
            }
			
        }
        else if (strId.indexOf('page4') != -1) //We're on page4
        {
            HideNext();
            ShowBack();
            ShowSend();
            
            var utcSendTimeSpan = $('span[id$="SendEmailCampaignWizard_SummarySendTime"]');
            if ($(utcSendTimeSpan).text() != 'Immediately')
            {
                var dateTimeParts = $(utcSendTimeSpan).text().split('T');
                var dateParts = dateTimeParts[0].split('-');
                var timeParts = dateTimeParts[1].split(':');
                
                var sendDateTime = new Date();
                sendDateTime.setUTCFullYear(dateParts[0], dateParts[1] - 1, dateParts[2]);
                sendDateTime.setUTCHours(timeParts[0]);
                sendDateTime.setUTCMinutes(timeParts[1]);
                sendDateTime.setUTCSeconds(0);
                $(utcSendTimeSpan).html(sendDateTime.toLocaleString());
            }
            
        }
        else if (strId.indexOf('page5') != -1) //We're on page5
        {
            HideNext();
            HideBack();
            HideSend();
        }
        else //We're somewhere else
        {
            EnableNext();
            ShowNext();
            ShowBack();
            HideSend();
        }
    });
};

function EnableNext() {
    var nextButton = $('input[id$="SendEmailCampaignWizard_btnNext"]');
    $(nextButton).removeAttr('disabled');
};

function DisableNext() {
    var nextButton = $('input[id$="SendEmailCampaignWizard_btnNext"]');
    $(nextButton).attr('disabled', 'disabled');
};

function HideBack() {
    var backButton = $('input[id$="SendEmailCampaignWizard_btnBack"]');
    $(backButton).css({'display':'none'});
};

function ShowBack() {
    var backButton = $('input[id$="SendEmailCampaignWizard_btnBack"]');
    $(backButton).css({'display':'inline'});
};

function HideSend() {
    var sendButton = $('input[id$="SendEmailCampaignWizard_btnSendCampaign"]');
    $(sendButton).css({'display':'none'});
};

function ShowSend() {
    var sendButton = $('input[id$="SendEmailCampaignWizard_btnSendCampaign"]');
    $(sendButton).css({'display':'inline'});
};

function HideNext() {
    var nextButton = $('input[id$="SendEmailCampaignWizard_btnNext"]');
    $(nextButton).css({'display':'none'});
};

function ShowNext() {
    var nextButton = $('input[id$="SendEmailCampaignWizard_btnNext"]');
    $(nextButton).css({'display':'inline'});
};

function EnableSendTime () {
    var sendTimeInput = $('span[id$="SendEmailCampaignWizard_dntSendTime"] input');
    var sendTimeImgLink = $('span[id$="SendEmailCampaignWizard_dntSendTime"] a');
    $(sendTimeInput).removeAttr('disabled');
    $(sendTimeImgLink).css('display', 'inline');
};

function DisableSendTime() {
    var sendTimeInput = $('span[id$="SendEmailCampaignWizard_dntSendTime"] input');
    var sendTimeImgLink = $('span[id$="SendEmailCampaignWizard_dntSendTime"] a');
    $(sendTimeInput).attr('disabled', 'disabled');
    $(sendTimeImgLink).css('display', 'none');
};

function FormatSummarySendTime(sendTime, dateString) {
    $('span#summarySendTime').html('');
    var htmlString = "";
    if (sendTime == 'Immediately')
    {
        htmlString += $('span#immediately').html();
    }
    else if (sendTime == 'Scheduled')
    {
        htmlString += $('span#scheduled').html().format(dateString);
    }
    
    $('span#summarySendTime').html(htmlString);
};

function FormatSummarySplit() {
    $('span#summarySplit').html('');
    var htmlString = "";
    if ($('div[id$="SendEmailCampaignWizard_divSplitTest"]').length > 0) //is split
    {
        var messageString = $('span#split').html();
        var uniqueTargets = $('span#totalUniqueTargets').html();
        var splitPercent = $('input[id$="SendEmailCampaignWizard_txtTargetPercentage"]').val();
        var waitHours = $('input[id$="SendEmailCampaignWizard_txtWaitHours"]').val();
        var splitTargets = Math.round((splitPercent / 100) * uniqueTargets);
        var remainingTargets = (uniqueTargets - splitTargets);

        if (messageString != null)
        {
            htmlString += messageString.format(uniqueTargets, splitTargets, waitHours, remainingTargets);
        }
    }
    else //not split
    {
        var messageString = $('span#notSplit').html();
        var uniqueTargets = $('span#totalUniqueTargets').html();
        if (messageString != null)
        {
            htmlString += messageString.format(uniqueTargets);
        }
        //htmlString += 'not split';
    }
    $('span#summarySplit').html(htmlString);
};

function DisablePercentage()
{
    var percentageTextBox = $('input[id$="SendEmailCampaignWizard_txtTargetPercentage"]');
    $(percentageTextBox).attr('readonly', 'readonly');
}

String.prototype.format = function(i, safe, arg) {

  function format() {
    var str = this, len = arguments.length+1;

    // For each {0} {1} {n...} replace with the argument in that position.  If 
    // the argument is an object or an array it will be stringified to JSON.
    for (i=0; i < len; arg = arguments[i++]) {
      safe = typeof arg === 'object' ? JSON.stringify(arg) : arg;
      str = str.replace(RegExp('\\{'+(i-1)+'\\}', 'g'), safe);
    }
    return str;
  }

  // Save a reference of what may already exist under the property native.  
  // Allows for doing something like: if("".format.native) { /* use native */ }
  format.native = String.prototype.format;

  // Replace the prototype property
  return format;

}();
</script>