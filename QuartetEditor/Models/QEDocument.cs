using Prism.Mvvm;
using QuartetEditor.Entities;
using QuartetEditor.Enums;
using QuartetEditor.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Models
{
    /// <summary>
    /// QuartetEditorドキュメントクラス
    /// </summary>
    public class QEDocument : BindableBase, IDisposable
    {
        /// <summary>
        /// 破棄用
        /// </summary>
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        /// <summary>
        /// ユーザーへのエラー通知要求
        /// </summary>
        public IObservable<string> ShowErrorMessageRequest => _ShowErrorMessageRequest;

        private Subject<string> _ShowErrorMessageRequest = new Subject<string>();

        /// <summary>
        /// システムのデータ
        /// </summary>
        public static QEDocument Current { get; } = new QEDocument();

        /// <summary>
        /// 文書のテキスト
        /// </summary>
        public NodeManager Content { get; } = new NodeManager();

        /// <summary>
        /// QuartetEditorドキュメントクラス
        /// </summary>
        public QEDocument()
        {
            this.Content.NodeEdited.Subscribe(v => this.IsEdited = v);
        }

        /// <summary>
        /// QuartetEditorドキュメントクラス
        /// </summary>
        static QEDocument()
        {
#if DEBUG
            var desctiption = new QuartetEditorDescription();
            desctiption.Node.Add(new QuartetEditorDescriptionItem() { Name = "ノード１", Content = "ノード１" });
            desctiption.Node[0].Children.Add(new QuartetEditorDescriptionItem() { Name = "ノード１-１", Content = "ノード１-１" });
            desctiption.Node[0].Children.Add(new QuartetEditorDescriptionItem() { Name = "ノード１-２", Content = "ノード１-２" });
            desctiption.Node[0].Children[1].Children.Add(new QuartetEditorDescriptionItem() { Name = "ノード１-２-１", Content = "ノード１-２-１" });
            desctiption.Node[0].Children[1].Children.Add(new QuartetEditorDescriptionItem() { Name = "ノード１-２-２", Content = "ノード１-２-２" });
            desctiption.Node.Add(new QuartetEditorDescriptionItem() { Name = "ノード２", Content = "ノード２" });
            desctiption.Node[1].Children.Add(new QuartetEditorDescriptionItem() { Name = "ノード２-１", Content = "ノード２-１" });
            desctiption.Node[1].Children[0].Children.Add(new QuartetEditorDescriptionItem() { Name = "ノード２-１-１", Content = "ノード２-１-１" });
            desctiption.Node[1].Children[0].Children.Add(new QuartetEditorDescriptionItem() { Name = "ノード２-１-２", Content = "ノード２-１-２" });
            desctiption.Node[1].Children.Add(new QuartetEditorDescriptionItem() { Name = "ノード２-２", Content = "ノード２-２" });
            desctiption.Node[1].Children[1].Children.Add(new QuartetEditorDescriptionItem() { Name = "ノード２-２-１", Content = "ノード２-２-１" });
            desctiption.Node[1].Children[1].Children.Add(new QuartetEditorDescriptionItem() { Name = "ノード２-２-２", Content = "ノード２-２-２" });
            desctiption.Node.Add(new QuartetEditorDescriptionItem() { Name = "ノード３", Content = "ノード３" });

            Current.Content.Load(desctiption);

            Current.IsEdited = false;
#endif
        }


#region File

        /// <summary>
        /// ViewへのSavePath処理要求
        /// </summary>
        public IObservable<Action<string>> SavePathRequest => _SavePathRequest;

        private Subject<Action<string>> _SavePathRequest = new Subject<Action<string>>();

        /// <summary>
        /// ViewへのOpenPath処理要求
        /// </summary>
        public IObservable<Action<string>> OpenPathRequest => _OpenPathRequest;

        private Subject<Action<string>> _OpenPathRequest = new Subject<Action<string>>();

        /// <summary>
        /// 編集されたか
        /// </summary>
        private bool _IsEdited = false;

        public bool IsEdited
        {
            get { return this._IsEdited; }
            set { this.SetProperty(ref this._IsEdited, value); }
        }

        /// <summary>
        /// 開いているファイルのタイプ
        /// </summary>
        private FileType? Type { get; set; }

        /// <summary>
        /// ファイル名のフルパス
        /// </summary>
        private string _FilePath;

        public string FilePath
        {
            get { return this._FilePath; }
            set { this.SetProperty(ref this._FilePath, value); }
        }

        /// <summary>
        /// ファイルを上書き保存する
        /// </summary>
        /// <returns></returns>
        public bool SaveOverwrite()
        {
            if (this.FilePath == null ||
                string.IsNullOrWhiteSpace(this.FilePath) ||
                !File.Exists(this.FilePath))
            {
                // 有効なファイル名が存在しない場合は変名処理へ
                return this.SaveAs();
            }

            if (this.Save(this.FilePath))
            {
                this.Content.OffEditFlag();
                this.IsEdited = false;
                return true;
            }
            else
            {
                this._ShowErrorMessageRequest.OnNext("ファイルの保存に失敗しました。\n別の場所に保存してください。");
            }

            return false;
        }

        /// <summary>
        /// ファイルを変名保存する
        /// </summary>
        /// <returns></returns>
        public bool SaveAs()
        {
            bool result = false;
            this._SavePathRequest.OnNext(path =>
            {
                if (path != null && !string.IsNullOrWhiteSpace(path))
                {
                    if (this.Save(path, FileType.QEDocument))
                    {
                        this.FilePath = path;
                        this.Type = FileType.QEDocument;
                        this.Content.OffEditFlag();
                        this.IsEdited = false;
                        result = true;
                    }
                    else
                    {
                        this._ShowErrorMessageRequest.OnNext("ファイルの保存に失敗しました。");
                    }
                }
            });
            return result;
        }

        /// <summary>
        /// ファイルを保存する
        /// </summary>
        public bool Save(string path)
        {
            return this.Save(path, this.Type ?? FileType.QEDocument);
        }
        /// <summary>
        /// ファイルを保存する
        /// </summary>
        private bool Save(string path, FileType type)
        {
            try
            {
                bool overwrite = File.Exists(path);
                bool result = false;
                string overwiteSuffix = Path.GetRandomFileName().Substring(0, 5);
                if (overwrite)
                {
                    File.Move(path, path + "." + overwiteSuffix);
                }

                switch (type)
                {
                    case FileType.QEDocument:
                        {
                            var data = new QuartetEditorDescription(this.Content.Tree);
                            result = FileUtility.SaveJsonObject(path, data);
                        }
                        break;
                    case FileType.TreeText:
                        {
                            var data = NodeConverterUtility.ToTreeText(new QuartetEditorDescription(this.Content.Tree));
                            result = FileUtility.SaveText(path, data, Encoding.UTF8);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if (result && overwrite)
                {
                    File.Delete(path + "." + overwiteSuffix);
                }
                return result;
            }
            catch
            {

            }
            return false;
        }

        /// <summary>
        /// ファイルを開いて読み込む
        /// </summary>
        /// <returns></returns>
        public void OpenQED()
        {
            this._OpenPathRequest.OnNext(path =>
            {
                this.Load(path);
            });
        }

        /// <summary>
        /// ファイルの読み込みを行います
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            if (File.Exists(path))
            {
                if (this.IsEdited)
                {
                    this.OpenNewProcess(path);
                    return;
                }

                FileType? fileType = null;
                try
                {
                    QuartetEditorDescription model;
                    if (FileUtility.LoadJsonObject(path, out model) == true)
                    {
                        // QEDファイルとして読み込み成功
                        fileType = FileType.QEDocument;
                    }

                    if (!fileType.HasValue)
                    {
                        string treeText;
                        if (FileUtility.LoadTextByAnyEncoding(path, out treeText) == true &&
                            NodeConverterUtility.FromTreeText(treeText, out model) == true)
                        {
                            // 階層付きテキストファイルとして読み込み成功
                            fileType = FileType.TreeText;
                        }

                    }

                    if (fileType.HasValue)
                    {
                        // 失敗していない場合、QuartetEditorDescriptionをNodeとして設定
                        this.Content.Load(model);
                        this.IsEdited = false;
                        this.FilePath = path;
                        this.Type = fileType.Value;
                    }
                }
                catch
                {

                }

                if (!fileType.HasValue)
                {
                    this._ShowErrorMessageRequest.OnNext("ファイルの読み込みに失敗しました。");
                }
            }
        }

        /// <summary>
        /// 新しいプロセスでファイルを開く
        /// </summary>
        /// <param name="path"></param>
        private void OpenNewProcess(string path)
        {
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location, "\"" + path + "\"");
        }

#endregion File

#region Export

        /// <summary>
        /// Viewへのエクスポート先SavePath処理要求
        /// </summary>
        public IObservable<Tuple<string, string, Action<string>>> ExportSavePathRequest => _ExportSavePathRequest;

        private Subject<Tuple<string, string, Action<string>>> _ExportSavePathRequest = new Subject<Tuple<string, string, Action<string>>>();


        /// <summary>
        /// エクスポート
        /// </summary>
        public void Export(ExportSettingModel model)
        {
            bool fail = false;
            string ext = "";
            string filter = "";
            string exportstr = "";
            try
            {
                switch (model.Kind)
                {
                    case ExportKind.Text:
                        exportstr = NodeConverterUtility.ToText(new QuartetEditorDescription(this.Content.Tree), model);
                        ext = "txt";
                        filter = "テキストファイル(*.txt)|*.txt|全てのファイル(*.*)|*.*";
                        break;
                    case ExportKind.HTML:
                        exportstr = NodeConverterUtility.ToHTML(new QuartetEditorDescription(this.Content.Tree), Path.GetFileNameWithoutExtension(this.FilePath));
                        ext = "html";
                        filter = "HTMLファイル(*.html)|*.html|全てのファイル(*.*)|*.*";
                        break;
                    case ExportKind.TreeText:
                        exportstr = NodeConverterUtility.ToTreeText(new QuartetEditorDescription(this.Content.Tree));
                        ext = "txt";
                        filter = "テキストファイル(*.txt)|*.txt|全てのファイル(*.*)|*.*";
                        break;
                    default:
                        throw new NotImplementedException();
                }

                // 保存する
                this._ExportSavePathRequest.OnNext(new Tuple<string, string, Action<string>>(filter, ext, (path) =>
                {
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        fail = !FileUtility.SaveText(path, exportstr, Encoding.UTF8);
                    }
                    return;
                }));
            }
            catch (Exception)
            {
                fail = true;
            }

            if (fail)
            {
                this._ShowErrorMessageRequest.OnNext("エクスポートに失敗しました…");
            }
        }

#endregion Export

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            this.Disposable.Dispose();
            this.Content.Dispose();
        }

#region Enum

        /// <summary>
        /// ファイルの種別
        /// </summary>
        enum FileType
        {
            /// <summary>
            /// QuartetEditorDocument形式
            /// </summary>
            QEDocument,

            /// <summary>
            /// 階層付きテキスト形式
            /// </summary>
            TreeText,
        }
#endregion
    }
}
