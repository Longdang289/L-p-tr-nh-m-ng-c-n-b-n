using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mural_001;
public class Room // Đại diện cho một phòng, chứa nhiều người dùng
{
    public int roomID; // ID mỗi phòng
    public List<User> userList = new List<User>(); // Danh sách các người dùng hiện tại trong phòng
    public List<Drawing> Drawings { get; private set; } // Danh sách các nét vẽ
    public string jsondrawlist; // Chuỗi JSON lưu trữ toàn bộ danh sách Drawing
    public List<Image> images { get; private set; }
    public string jsonimagelist;
    public int imageCount { get; private set; }
    public List<sticky_note> notes { get; private set; }
    public string jsonnotelist;
    public int noteCount;
    public string jsonforceview;
    public bool viewState;
    public Room()
    {
        Drawings = new List<Drawing>();
        jsondrawlist = null; // Ban đầu, jsondrawlist được đặt thành null nếu danh sách Drawing trống
        images= new List<Image>();
        jsonimagelist = null;
        imageCount = 0;
        notes = new List<sticky_note>();
        jsonnotelist = null;
        noteCount = 0;
        jsonforceview = null;
        viewState = false;
    }
    
    public void ImageUpdate(string a)
    {
        // Giải mã JSON thành đối tượng Image mới
        Image newImage = JsonConvert.DeserializeObject<Image>(a);

        // Tìm đối tượng trong danh sách theo ImageID
        var existingImage = images.FirstOrDefault(img => img.ImageID == newImage.ImageID);

        if (existingImage != null)
        {
            // Cập nhật các thuộc tính cần thiết của đối tượng hiện có
            existingImage.Position = newImage.Position;
            existingImage.ImageSize = newImage.ImageSize;
            existingImage.content = newImage.content;

            // Cập nhật jsonimagelist sau khi chỉnh sửa
            UpdateJsonImageList();
            Debug.WriteLine($"ImageID {existingImage.ImageID} đã được cập nhật: Position = {existingImage.Position}, Size = {existingImage.ImageSize}");
        }
        else
        {
            Debug.WriteLine($"Không tìm thấy Image với ImageID: {newImage.ImageID} trong danh sách.");
        }
    }
    public void NoteUpdate(string a)
    {
        // Giải mã JSON thành đối tượng note mới
        sticky_note newnote = JsonConvert.DeserializeObject<sticky_note>(a);

        // Tìm đối tượng trong danh sách theo noteID
        var existingnote = notes.FirstOrDefault(nt => nt.noteID == newnote.noteID);

        if (existingnote != null)
        {
            // Cập nhật các thuộc tính cần thiết của đối tượng hiện có
            existingnote.NoteText = newnote.NoteText;
            existingnote.NoteSize = newnote.NoteSize;
            existingnote.LocationPoint = newnote.LocationPoint;
            existingnote.BackgroundColor=newnote.BackgroundColor;

            // Cập nhật jsonnotelist sau khi chỉnh sửa
            UpdateJsonNoteList();
            //Debug.WriteLine($"ImageID {existingImage.ImageID} đã được cập nhật: Position = {existingImage.Position}, Size = {existingImage.ImageSize}");
        }
        else
        {
            Debug.WriteLine($"Không tìm thấy note với noteID: {newnote.noteID} trong danh sách.");
        }
    }
    // Thêm một nét vẽ vào danh sách và cập nhật jsondrawlist
    public void AddDrawing(Drawing drawing)
    {
        Drawings.Add(drawing);
        UpdateJsonDrawList(); // Cập nhật jsondrawlist mỗi khi thêm Drawing mới
    }
    public void AddImage(Image image)
    {
        imageCount++;
        image.ImageID = imageCount;
        images.Add(image);
        UpdateJsonImageList();
    }
    public void AddNote(sticky_note note)
    {
        noteCount++;
        note.noteID = noteCount;
        notes.Add(note);
        UpdateJsonNoteList();
    }
    public void AddNewDrawingByJSON(string a)
    {
        Drawing newDrawing = JsonConvert.DeserializeObject < Drawing > (a);
        Drawings.Add(newDrawing);
        UpdateJsonDrawList(); // Cập nhật jsondrawlist mỗi khi thêm Drawing mới
    }
    public void AddImageByJSON(string a)
    {
        imageCount++;
        Image newImage = JsonConvert.DeserializeObject< Image >(a);
        newImage.ImageID=imageCount;
        images.Add(newImage);
        UpdateJsonImageList() ;
    }
    public void AddNoteByJSON(string a)
    {
        noteCount++;
        sticky_note newnote= JsonConvert.DeserializeObject< sticky_note >(a);
        newnote.noteID=noteCount;
        notes.Add(newnote);
        UpdateJsonNoteList() ;
    }
    // Cập nhật chuỗi JSON cho toàn bộ danh sách Drawings
    private void UpdateJsonDrawList()
    {
        jsondrawlist = Drawings.Count > 0 ? JsonConvert.SerializeObject(Drawings) : null;
    }
    private void UpdateJsonImageList()
    {
        jsonimagelist = images.Count > 0 ? JsonConvert.SerializeObject(images) : null;
    }
    private void UpdateJsonNoteList()
    {
        jsonnotelist = notes.Count > 0 ? JsonConvert.SerializeObject(notes) : null;
    }
    // Phương thức để lấy chuỗi JSON hiện tại của danh sách các nét vẽ
    public string GetDrawingsAsJson()
    {
        return jsondrawlist;
    }
    public string GetImagesAsJson()
    {
        return jsonimagelist;
    }
    public string GetNotesAsJson()
    {
        return jsonnotelist;
    }
    public string GetUsernameListInString() // Trả về list tên người dùng trong phòng, ngăn cách bởi dấu phẩy
    {
        List<string> usernames = new List<string>();
        foreach (User user in userList)
        {
            usernames.Add(user.Username);
        }
        return string.Join(",", usernames.ToArray());
    }
}

