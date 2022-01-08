let index = 0;

function AddTag() {

    const swalWithDarkButton = Swal.mixin({
        customClass: {
            confirmButton: "btn btn-danger btn-md",
        },
        icon: 'error',
        timer: 3000,
        buttonsStyling: false
    });

    var tagEntry = document.getElementById("TagEntry");

    //Use the search function to detect error cases
    let searchResult = Search(tagEntry.value);

    if (searchResult != null) {
        //Trigger SweetAlert if result passed from Search()
        swalWithDarkButton.fire({
            html: `${searchResult}`
        })
    }
    else {
        //create a new select option
        let newOption = new Option(tagEntry.value, tagEntry.value);
        document.getElementById("tagList").options[index++] = newOption;
    }

    //Clear out tag textbox
    tagEntry.value = "";
    return true;
}

function RemoveTag() {
    const swalWithDarkButton2 = Swal.mixin({
        customClass: {
            confirmButton: "btn btn-danger btn-sm",
        },
        imageUrl: "./images/user-placeholder.png",
        timer: 3000,
        buttonsStyling: false
    });

    let tagCount = 1;

    let tagList = document.getElementById("tagList");

    if (!tagList) return false;
    if (tagList.selectedIndex == -1) {
        swalWithDarkButton2.fire ({
            html: 'Choose a tag before deleting'
        })
        return true;
    }

    while (tagCount > 0) {

        if (tagList.selectedIndex >= 0) {
            tagList.options[tagList.selectedIndex] = null;
            --tagCount;
        }
        else 
            tagCount = 0;
        index--;
    }
}

//Detects either an empty or a duplicate tag within the post and return an error string if not
function Search(tagStr) {
    if (tagStr == "") {
        return "Empty tags are not permitted.";
    }

    tagsEl = document.getElementById("tagList");
    if (tagsEl) {
        let options = tagsEl.options; //Get all the tags from the "tagList" select element's <option> elements

        for (let index = 0; index < options.length; index++) {
            if (options[index].value == tagStr) {
                return `Tag #${tagStr} already exists!`;
            }
        }
    }
}

//Create a new Option(list item) to replace a tag at the specified index
function ReplaceTag(tag, index) {
    let newOption = new Option(tag, tag);
    document.getElementById("tagList").options[index] = newOption;
}

//look for the tagValues var to see if it has data
if (tagValues != '') {
    let tagArray = tagValues.split(",");
    for (let loop = 0; loop < tagArray.length; loop++) {
        //Load up or replace the options that we have
        ReplaceTag(tagArray[loop], loop);
        index++;
    }
}

/** NOTES
 * $("form") 
 *   - JQuery selector which selects the form element
 *   
 * $("form").on("submit", function () {} ) 
 *   -Adds a submit event handler to the selected form element 
 *   -defines function to fire when the form is submitted
 *   
 * $("#tagList option")
 *   -Selcts all options from the tagList <select> element 
 *   -Selection can be specified; Ex: $("#tagList option:first")
 *   
 * $("#tagList option").prop("selected", "selected");
 *   -Gathers the "selected" property for each <option> tag in 
 *    #tagList and assigns "selected" as the value
 *    
 *      Ex: <select>
 *            <option selected>Tag</option>
 *          </select> 
 **/
//Create tagList element and attaches submit event handler to pass to controller (and select all?)
$("form").on("submit", function () {
    $("#tagList option").prop("selected", "selected");
})