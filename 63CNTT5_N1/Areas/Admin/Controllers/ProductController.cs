using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyClass.Model;
using MyClass.DAO;
using UDW.Library;
using System.IO;

namespace _63CNTT5_N1.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        ProductsDAO productDAO = new ProductsDAO();
        CategoriesDAO categoriesDAO = new CategoriesDAO();
        SuppliersDAO suppliersDAO = new SuppliersDAO();
        // GET: Admin/product/index
        //Index
        public ActionResult Index()
        {
            return View(productDAO.getList("Index"));
        }

        // GET: Admin/Suppliers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //thông báo thất bại

                TempData["message"] = new XMessage("danger", "Không tồn tại nhà cung cấp");
                return RedirectToAction("Index");
            }
            Products products = productDAO.getRow(id);
            if (products == null)
            {
                //thông báo thất bại

                TempData["message"] = new XMessage("danger", "Không tồn tại nhà cung cấp");
                return RedirectToAction("Index");

            }
            return View(products);
        }
        ///////
        // GET: Admin/Suppliers/Create
        public ActionResult Create()
        {
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");//sai CatID - truy van tu bang catelogi

            ViewBag.ListSupID = new SelectList(suppliersDAO.getList("Index"), "Id", "Name"); // sai Suplier - truy van bang suplier
            //dung de lua chon danh sach droplist nhu bang catelogies: parentId va Supplier: ParentID


            return View();
        }
        // POST: Admin/Suppliers/Create/5       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Products product)
        {
            if (ModelState.IsValid)
            {
                //xử lý tự động slug,createAt,BY, updateAt,BY,Order

                //Xử lý tự động: CreateAt
                product.CreateAt = DateTime.Now;
                //Xử lý tự động: UpdateAt
                product.UpdateAt = DateTime.Now;

                product.CreateBy = Convert.ToInt32(Session["UserId"]);

                product.UpdateBy = Convert.ToInt32(Session["UserId"]);


                //Xử lý tự động: Slug
                product.Slug = XString.Str_Slug(product.Name);

                //xu ly cho phan upload hình ảnh
                var img = Request.Files["img"];//upload hinh
                string PathDir = "~/Public/img/product";//cập nhập hình
                if (img != null && img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = product.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        product.Img = imgName;

                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                    //(phần này nằm ở edit thực hiện chức năng cập nhập lại hình ảnh

                    ////xử lý cho mục xóa hình ảnh
                    //if (suppliers.Image != null)
                    //{
                    //    string DelPath = Path.Combine(Server.MapPath(PathDir), suppliers.Image);
                    //    System.IO.File.Delete(DelPath);
                    //}
                    //)

                }//ket thuc phan upload hinh anh




                //chèn mẫu tin vào database;
                productDAO.Insert(product);
                //thông báo mẫu tin thành công
                TempData["message"] = new XMessage("success", "Tạo mới sản phẩm thành công");
                return RedirectToAction("Index");

            }
            ViewBag.ListCatId = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");//sai CatID - truy van tu bang catelogi

            ViewBag.ListSupID = new SelectList(suppliersDAO.getList("Index"), "Id", "Name"); // sai Suplier - truy van bang suplier
            return View(product);
        }

        // GET: Admin/Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {

                //thông báo thất bại

                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            Products product = productDAO.getRow(id);
            if (product == null)
            {

                //thông báo thất bại

                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            ViewBag.ListCatId = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");//sai CatID - truy van tu bang catelogi

            ViewBag.ListSupID = new SelectList(suppliersDAO.getList("Index"), "Id", "Name"); // sai Suplier - truy van bang suplier

            return View(product);
        }
        // POST: Admin/Suppliers/Edit/5       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Products products)
        {
            if (ModelState.IsValid)
            {
                // xử lý tự động slug,, BY, updateAt, BY, Order


                //Xử lý tự động: UpdateAt
                products.UpdateAt = DateTime.Now;

                //Xử lý tự động: Slug
                products.Slug = XString.Str_Slug(products.Name);

                //Trước khi cập nhập hình ảnh thì xóa ảnh cũ
                var img = Request.Files["img"];//upload hinh
                string PathDir = "~/Public/img/product";//cập nhập hình
                //xử lý xóa ảnh cũ
                if (img.ContentLength != 0 && products.Img != null)//tồn tại 1 logo của NCC từ trước
                {
                    string DelPath = Path.Combine(Server.MapPath(PathDir), products.Img);
                    System.IO.File.Delete(DelPath);
                }
                //Cập nhập ảnh mới của Nhà cung cấp


                    if (img != null && img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = products.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        products.Img = imgName;

                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }

                }//ket thuc phan upload hinh anh


                //cập nhập mãu tin vào database
                productDAO.Update(products);
                //thông báo mẫu tin thành công
                TempData["message"] = new XMessage("success", "cập nhập sản phẩm thành công");
                return RedirectToAction("Index");
            }
            //  ViewBag.ListOrder = new SelectList(productDAO.getList("Index"), "Order", "Name");

            return View(products);
        }

        // GET: Admin/Suppliers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            Products product = productDAO.getRow(id);
            if (product == null)
            {
                //thông báo thất bại
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products product = productDAO.getRow(id);
            productDAO.Delete(product);

            //xóa mẫu tin thành công
            TempData["message"] = new XMessage("success", "Xóa sản phẩm thành công");

            return RedirectToAction("Trash");
        }

        //phát sinh thêm hàm mới: status, Trash,deltrash,undo

        /// ///////////////////////////////////////////////////
        /// Thùng rác - Trash
        // GET: Admin/Category/Status/5
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //Thông báo cập nhập trạng thái thất bại

                TempData["message"] = new XMessage("danger", "Cập nhập trạng thái thất bại");
                return RedirectToAction("Index");
            }
            //Truy vấn id
            Products products = productDAO.getRow(id);
            if (products == null)
            {
                //Thông báo cập nhập trạng thái thất bại

                TempData["message"] = new XMessage("danger", "Cập nhập trạng thái thất bại");
                return RedirectToAction("Index");
            }
            else
            {


                //chuyển đổi trạng thái của Status 1<->2
                products.Status = (products.Status == 1) ? 2 : 1;
                //Cập nhập trạng thái
                products.UpdateAt = DateTime.Now;
                //cập nhập lại database
                productDAO.Update(products);

                //Thông báo cập nhập trạng thái thành công
                TempData["message"] = TempData["message"] = new XMessage("success", "Cập nhập trạng thái thành công");
                return RedirectToAction("Index");
            }
        }


        // GET: Admin/Category/DelTrash/5
        public ActionResult DelTrash(int? id)
        {
            if (id == null)
            {
                //Thông báo cập nhập trạng thái thất bại

                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            //Truy vấn id
            Products product = productDAO.getRow(id);
            if (product == null)
            {
                //Thông báo cập nhập trạng thái thất bại

                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            else
            {


                //chuyển đổi trạng thái của Status 1,2<->0 : khong hieu thi o index
                product.Status = 0;
                //Cập nhập trạng thái
                product.UpdateAt = DateTime.Now;
                //cập nhập lại database
                productDAO.Update(product);

                //Thông báo cập nhập trạng thái thành công
                TempData["message"] = TempData["message"] = new XMessage("success", "Xóa mẫu tin thành công");
                return RedirectToAction("Index");
            }
        }

        /// Trash
        // GET: Admin/Category/Recover
        public ActionResult Trash()
        {
            return View(productDAO.getList("Trash"));
        }
        //Recover
        // GET: Admin/Category/Recover/5

        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                //Thông báo cập nhập trạng thái thất bại

                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            //Truy vấn id
            Products product = productDAO.getRow(id);
            if (product == null)
            {
                //Thông báo cập nhập trạng thái thất bại

                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            else
            {


                //chuyển đổi trạng thái của Status 0-> 2: không xuất bản
                product.Status = 2;
                //Cập nhập trạng thái
                product.UpdateAt = DateTime.Now;
                //cập nhập lại database
                productDAO.Update(product);

                //Thông báo phục hồi mẫu tin thành công
                TempData["message"] = TempData["message"] = new XMessage("success", "Phục hồi mẫu tin thành công");
                return RedirectToAction("Index");
            }
        }
    }
}
